using Xunit;
using System.Linq;
using System.Text.Json;
using fs_2025_assessment_1_71617.Models;
using fs_2025_assessment_1_71617.Services;

namespace BikeStation.Tests
{
    public class StationServiceTests
    {
        private static void WriteStationsJson(params Station[] stations)
        {
            var dataDir = System.IO.Path.Combine(AppContext.BaseDirectory, "Data");
            System.IO.Directory.CreateDirectory(dataDir);

            var jsonPath = System.IO.Path.Combine(dataDir, "dublinbike.json");
            var json = JsonSerializer.Serialize(stations);
            System.IO.File.WriteAllText(jsonPath, json);
        }

        [Fact]
        public void GetStations_FiltersByStatus_Open()
        {
            WriteStationsJson(
                new Station
                {
                    Number = 1,
                    Name = "A",
                    Address = "A",
                    Status = "OPEN",
                    Available_Bikes = 5,
                    Bike_Stands = 10,
                    Position = new GeoPosition { Lat = 53.0, Lng = -6.0 }
                },
                new Station
                {
                    Number = 2,
                    Name = "B",
                    Address = "B",
                    Status = "CLOSED",
                    Available_Bikes = 0,
                    Bike_Stands = 10,
                    Position = new GeoPosition { Lat = 53.1, Lng = -6.1 }
                }
            );

            var service = new StationService();

            var options = new StationQueryOptions
            {
                Status = "OPEN"
            };

            var results = service.GetStations(options, out int totalCount).ToList();

            Assert.Equal(1, totalCount);
            Assert.Single(results);
            Assert.Equal("OPEN", results[0].Status, ignoreCase: true);
        }
    }
}
