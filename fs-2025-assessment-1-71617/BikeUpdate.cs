using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using fs_2025_assessment_1_71617.Services;

namespace fs_2025_assessment_1_71617.Background
{
    public class BikeUpdateBackgroundService : BackgroundService
    {
        private readonly IStationService _stationService;
        private readonly Random _random = new();

        public BikeUpdateBackgroundService(IStationService stationService)
        {
            _stationService = stationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                UpdateStations();
                await Task.Delay(TimeSpan.FromSeconds(_random.Next(10, 21)), stoppingToken);
            }
        }

        private void UpdateStations()
        {
            var options = new StationQueryOptions { PageSize = 9999 };
            var stations = _stationService.GetStations(options, out _);

            foreach (var station in stations)
            {
                int newCapacity = _random.Next(10, 41);
                int newAvailableBikes = _random.Next(0, newCapacity + 1);

                station.Bike_Stands = newCapacity;
                station.Available_Bikes = newAvailableBikes;
                station.Available_Bike_Stands = newCapacity - newAvailableBikes;
                station.Last_Update = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                _stationService.Update(station.Number, station);
            }
        }
    }
}
