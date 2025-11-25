using System.Runtime.CompilerServices;

namespace WeatherStationDataServer.Models.DataModel;

/// <summary>
/// Represents a single device its id, location, time when last 
/// message was received and a list of weather parameters that 
/// it received
/// </summary>
public class WeatherDevice : ICacheManageable
{
    public string DeviceId { get; set; }
    public Geolocation.Coordinate Location { get; set; }
    public DateTime LastReceivalTime { get; set; }
    public Dictionary<string, WeatherParameters> Parameters { get; set; }

    private long _dataCount = 15;
    public long DataCount 
    { 
        get =>  _dataCount;
        set => _dataCount = value switch
        {
            0 => throw new ArgumentNullException(nameof(value)),
            _ => value
        };
    }

    public WeatherDevice(string id, Geolocation.Coordinate location, Dictionary<string, WeatherParameters> weatherData)
    {
        DeviceId = id;
        Location = location;
        LastReceivalTime = DateTime.UtcNow;
        Parameters = weatherData;
    }

    public void ManageDataCount()
    {
        foreach (var kv in Parameters)
        {
            for (long i = 0; i < kv.Value.Values.Count - _dataCount; i++)
                kv.Value.Values.RemoveAt(0);
        }
    }
}

public sealed class WeatherDeviceBuilder 
{
    public static WeatherDeviceBuilder Instance { get; } = 
        new WeatherDeviceBuilder();
    private WeatherDeviceBuilder() {}

    private WeatherDevice? _buldingWeatherDevice = null;

    public WeatherDeviceBuilder DeviceParameters(string name, Geolocation.Coordinate coordinates) 
    {
        _buldingWeatherDevice = new WeatherDevice(name, coordinates, new Dictionary<string, WeatherParameters>());
        return this;
    }

    public WeatherDeviceBuilder AddWeatherParameter(string name, Enum unit) 
    {
        _buldingWeatherDevice?.Parameters.Add(
            name,
            new WeatherParameters(
                unit,
                new List<WeatherParameterValue> { })
        );
        return this;
    }

    public WeatherDevice Build() =>
        (_buldingWeatherDevice is null) ?
            throw new ArgumentException("Not all parameters were set") :
            _buldingWeatherDevice;
}