using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using fs_2025_assessment_1_71617.Models;
using fs_2025_assessment_1_71617.Services;
using fs_2025_assessment_1_71617.DIOs;

namespace fs_2025_assessment_1_71617.Controllers
{
    [ApiController]
    [Route("api/v1/stations")]    
    public class StationsV1Controller : ControllerBase
    {
        private readonly IStationService _service;
        private readonly IMemoryCache _cache;
        private readonly TimeZoneInfo _dublinTz;

        public StationsV1Controller(IStationService service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
            _dublinTz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Dublin");
        }

        [HttpGet]
        public ActionResult<IEnumerable<StationDto>> GetStations(
            [FromQuery] string? status,
            [FromQuery] string? q,
            [FromQuery] int? minBikes,
            [FromQuery] string? sort,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var options = new StationQueryOptions
            {
                Status = status,
                Q = q,
                MinBikes = minBikes,
                Sort = sort,
                Page = page,
                PageSize = pageSize
            };

            var cacheKey = "v1-stations-" + options.GetCacheKey();

            if (!_cache.TryGetValue(cacheKey, out List<StationDto>? result))
            {
                var stations = _service.GetStations(options, out _);
                result = stations
                    .Select(s => StationDto.FromModel(s, _dublinTz))
                    .ToList();

                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            }

            return Ok(result);
        }
        [HttpDelete("{number:int}")]
        public IActionResult Delete(int number)
        {
            var deleted = _service.Delete(number);

            if (!deleted)
                return NotFound(new { message = $"Station {number} not found" });

            _cache.Remove($"v1-station-{number}");
            _cache.Remove("v1-stations-summary");

            return NoContent();
        }

        [HttpGet("{number:int}")]
        public ActionResult<StationDto> GetByNumber(int number)
        {
            var cacheKey = $"v1-station-{number}";

            if (!_cache.TryGetValue(cacheKey, out StationDto? dto))
            {
                var station = _service.GetByNumber(number);
                if (station == null) return NotFound();

                dto = StationDto.FromModel(station, _dublinTz);
                _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(5));
            }

            return Ok(dto);
        }

        [HttpGet("summary")]
        public ActionResult<StationSummary> GetSummary()
        {
            const string cacheKey = "v1-stations-summary";
            if (!_cache.TryGetValue(cacheKey, out StationSummary? summary))
            {
                summary = _service.GetSummary();
                _cache.Set(cacheKey, summary, TimeSpan.FromMinutes(5));
            }

            return Ok(summary);
        }

        [HttpPost]
        public ActionResult<StationDto> Create([FromBody] Station station)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = _service.Create(station);
                var dto = StationDto.FromModel(created, _dublinTz);

                _cache.Remove("v1-stations-summary");

                return CreatedAtAction(nameof(GetByNumber),
                    new { number = dto.Number }, dto);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{number:int}")]
        public IActionResult Update(int number, [FromBody] Station station)
        {
            if (number != station.Number)
            {
                return BadRequest(new { message = "Number in URL must match station number in body." });
            }

            var success = _service.Update(number, station);
            if (!success) return NotFound();

            _cache.Remove($"v1-station-{number}");
            _cache.Remove("v1-stations-summary");

            return NoContent();
        }
    }
}
