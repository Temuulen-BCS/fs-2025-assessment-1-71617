using System.Net.Http.Json;
using BikeStation.UI.Models;

namespace BikeStation.UI.Services;

public class StationsApiClient
{
    private readonly HttpClient _http;

    public StationsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<StationDto>> GetStationsAsync(
        string? status = null,
        string? q = null,
        int? minBikes = null,
        string? sort = null,
        int page = 1,
        int pageSize = 20)
    {
        var qList = new List<string>();

        if (!string.IsNullOrWhiteSpace(status))
            qList.Add($"status={status}");

        if (!string.IsNullOrWhiteSpace(q))
            qList.Add($"q={q}");

        if (minBikes.HasValue)
            qList.Add($"minBikes={minBikes.Value}");

        if (!string.IsNullOrWhiteSpace(sort))
            qList.Add($"sort={sort}");

        if (page != 1)
            qList.Add($"page={page}");

        if (pageSize != 20)
            qList.Add($"pageSize={pageSize}");

        string queryString = qList.Count > 0 ? "?" + string.Join("&", qList) : "";

        var result = await _http.GetFromJsonAsync<List<StationDto>>($"api/v1/stations{queryString}");
        return result ?? new List<StationDto>();
    }

    public async Task<StationDto?> GetStationAsync(int number)
    {
        return await _http.GetFromJsonAsync<StationDto>($"api/v1/stations/{number}");
    }

    public async Task<StationSummary?> GetSummaryAsync()
    {
        return await _http.GetFromJsonAsync<StationSummary>("api/v1/stations/summary");
    }

    public async Task<StationDto?> CreateStationAsync(Station station)
    {
        var res = await _http.PostAsJsonAsync("api/v1/stations", station);
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<StationDto>();
    }

    public async Task<bool> UpdateStationAsync(int number, Station station)
    {
        var res = await _http.PutAsJsonAsync($"api/v1/stations/{number}", station);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteStationAsync(int number)
    {
        var res = await _http.DeleteAsync($"api/v1/stations/{number}");
        return res.IsSuccessStatusCode;
    }
}
