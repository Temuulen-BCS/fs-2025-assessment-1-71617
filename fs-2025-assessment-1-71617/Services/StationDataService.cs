using fs_2025_assessment_1_71617.Models;
using System.Text.Json;

namespace fs_2025_assessment_1_71617.Services
{
    public class StationDataService
    {
        private readonly List<Station> _stations;

        public StationDataService()
        {
            var json = File.ReadAllText("Data/dublinbike.json");
            _stations = JsonSerializer.Deserialize<List<Station>>(json);
        }

        public IEnumerable<Station> GetStations() => _stations;
        public Station? GetByNumber(int number) => _stations.FirstOrDefault(s => s.Number == number);
    }

}
