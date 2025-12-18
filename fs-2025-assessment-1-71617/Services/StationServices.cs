using System.Text.Json;
using fs_2025_assessment_1_71617.Models;

namespace fs_2025_assessment_1_71617.Services
{
    public class StationService : IStationService
    {
        private readonly List<Station> _stations = new();
        private readonly object _lock = new();

        public StationService()
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "dublinbike.json");
            if (File.Exists(jsonPath))
            {
                var json = File.ReadAllText(jsonPath);
                var items = JsonSerializer.Deserialize<List<Station>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (items != null)
                {
                    _stations = items;
                }
            }
        }

        public IEnumerable<Station> GetStations(StationQueryOptions options, out int totalCount)
        {
            IEnumerable<Station> query = _stations;

            if (!string.IsNullOrWhiteSpace(options.Status))
            {
                var status = options.Status.Trim().ToUpperInvariant();
                query = query.Where(s => s.Status.ToUpperInvariant() == status);
            }

            if (options.MinBikes.HasValue && options.MinBikes.Value > 0)
            {
                query = query.Where(s => s.Available_Bikes >= options.MinBikes.Value);
            }

            if (!string.IsNullOrWhiteSpace(options.Q))
            {
                var term = options.Q.Trim().ToLowerInvariant();
                query = query.Where(s =>
                    (!string.IsNullOrEmpty(s.Name) && s.Name.ToLower().Contains(term)) ||
                    (!string.IsNullOrEmpty(s.Address) && s.Address.ToLower().Contains(term)));
            }

            bool desc = string.Equals(options.Dir, "desc", StringComparison.OrdinalIgnoreCase);
            query = options.Sort?.ToLower() switch
            {
                "name" => desc ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
                "availablebikes" => desc ? query.OrderByDescending(s => s.Available_Bikes) : query.OrderBy(s => s.Available_Bikes),
                "occupancy" => desc
                    ? query.OrderByDescending(s => s.Occupancy)
                    : query.OrderBy(s => s.Occupancy),
                _ => query.OrderBy(s => s.Number) 
            };

            totalCount = query.Count();

            var page = options.Page < 1 ? 1 : options.Page;
            var pageSize = options.PageSize < 1 ? 20 : options.PageSize;

            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return query.ToList();
        }

        public Station? GetByNumber(int number)
        {
            return _stations.FirstOrDefault(s => s.Number == number);
        }

        public StationSummary GetSummary()
        {
            var totalStations = _stations.Count;
            var totalBikeStands = _stations.Sum(s => s.Bike_Stands);
            var totalAvailableBikes = _stations.Sum(s => s.Available_Bikes);
            var openCount = _stations.Count(s => s.Status.Equals("OPEN", StringComparison.OrdinalIgnoreCase));
            var closedCount = _stations.Count(s => s.Status.Equals("CLOSED", StringComparison.OrdinalIgnoreCase));

            return new StationSummary
            {
                TotalStations = totalStations,
                TotalBikeStands = totalBikeStands,
                TotalAvailableBikes = totalAvailableBikes,
                OpenCount = openCount,
                ClosedCount = closedCount
            };
        }

        public Station Create(Station station)
        {
            lock (_lock)
            {
                if (_stations.Any(s => s.Number == station.Number))
                    throw new InvalidOperationException($"Station {station.Number} already exists.");

                _stations.Add(station);
                return station;
            }
        }

        public bool Delete(int number)
        {
            lock (_lock)
            {
                var station = _stations.FirstOrDefault(s => s.Number == number);
                if (station == null) return false;

                _stations.Remove(station);
                return true;
            }
        }

        public bool Update(int number, Station updated)
        {
            lock (_lock)
            {
                var existing = _stations.FirstOrDefault(s => s.Number == number);
                if (existing == null) return false;

                existing.Name = updated.Name;
                existing.Address = updated.Address;
                existing.Position = updated.Position;
                existing.Bike_Stands = updated.Bike_Stands;
                existing.Available_Bike_Stands = updated.Available_Bike_Stands;
                existing.Available_Bikes = updated.Available_Bikes;
                existing.Status = updated.Status;
                existing.Last_Update = updated.Last_Update;

                return true;
            }
        }
    }
}
