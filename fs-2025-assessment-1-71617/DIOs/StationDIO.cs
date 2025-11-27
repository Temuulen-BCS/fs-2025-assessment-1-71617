using System;

namespace fs_2025_assessment_1_71617.DIOs
{
    public class StationDto
    {
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public double Lat { get; set; }
        public double Lng { get; set; }

        public int BikeStands { get; set; }
        public int AvailableBikeStands { get; set; }
        public int AvailableBikes { get; set; }
        public string Status { get; set; } = string.Empty;

        public double Occupancy { get; set; }

        public DateTimeOffset LastUpdateUtc { get; set; }
        public DateTimeOffset LastUpdateLocal { get; set; }

        public static StationDto FromModel(Models.Station station, TimeZoneInfo dublinTz)
        {
            var lastUpdateUtc = DateTimeOffset.FromUnixTimeMilliseconds(station.Last_Update);
            var lastUpdateLocal = TimeZoneInfo.ConvertTime(lastUpdateUtc, dublinTz);

            var occupancy = station.Bike_Stands == 0
                ? 0
                : (double)station.Available_Bikes / station.Bike_Stands;

            return new StationDto
            {
                Number = station.Number,
                Name = station.Name,
                Address = station.Address,
                Lat = station.Position?.Lat ?? 0,
                Lng = station.Position?.Lng ?? 0,
                BikeStands = station.Bike_Stands,
                AvailableBikeStands = station.Available_Bike_Stands,
                AvailableBikes = station.Available_Bikes,
                Status = station.Status,
                Occupancy = occupancy,
                LastUpdateUtc = lastUpdateUtc,
                LastUpdateLocal = lastUpdateLocal
            };
        }
    }
}

