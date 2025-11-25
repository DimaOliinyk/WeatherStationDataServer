namespace WeatherStationDataServer.Models.ReceiveDataModel;

public class ReceiveWeatherDeviceData
{    
    public string DeviceId { get; set; }
    public DateTime LastReceivalTime { get; set; }
    public Dictionary<string, double> Parameters { get; set; }
    
    public ReceiveWeatherDeviceData(string id, DateTime receivalTime, Dictionary<string, double> weatherData)
    {
        DeviceId = id;
        LastReceivalTime = receivalTime;
        Parameters = weatherData;
    }
    
}
