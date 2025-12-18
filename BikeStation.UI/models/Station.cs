namespace BikeStation.UI.Models;

public class GeoPosition
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class Station
{
    public int Number { get; set; }
    public string Contract_Name { get; set; } = "dublin";
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public GeoPosition Position { get; set; } = new();

    public int Bike_Stands { get; set; }
    public int Available_Bike_Stands { get; set; }
    public int Available_Bikes { get; set; }
    public string Status { get; set; } = "OPEN";

    public long Last_Update { get; set; }
}
