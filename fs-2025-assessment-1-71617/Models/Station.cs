namespace fs_2025_assessment_1_71617.Models
{
    public class Station
    {
        public int Number { get; set; }
        public string Contract_Name { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public required GeoPosition Position { get; set; }

        public int Bike_Stands { get; set; }
        public int Available_Bike_Stands { get; set; }
        public int Available_Bikes { get; set; }
        public string Status { get; set; }
        public long Last_Update { get; set; }

        public double Occupancy =>
            Bike_Stands == 0 ? 0 : (double)Available_Bikes / Bike_Stands;
    }

}
