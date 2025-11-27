using fs_2025_assessment_1_71617.Models;

namespace fs_2025_assessment_1_71617.Services
{
    public interface IStationService
    {
        IEnumerable<Station> GetStations(StationQueryOptions options, out int totalCount);
        Station? GetByNumber(int number);
        StationSummary GetSummary();
        Station Create(Station station);
        bool Update(int number, Station updated);
        bool Delete(int number);

    }

    public class StationSummary
    {
        public int TotalStations { get; set; }
        public int TotalBikeStands { get; set; }
        public int TotalAvailableBikes { get; set; }
        public int OpenCount { get; set; }
        public int ClosedCount { get; set; }
    }
}

