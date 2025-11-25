namespace WeatherStationDataServer.Models.DataModel;

/// <summary>
/// Represents one single value at one single point in time
/// </summary>
public class WeatherParameterValue
{
    public double Value { get; set; }
    public DateTime TimeOfReading { get; set; }

    public WeatherParameterValue(double value, DateTime timeOfReading)
    {
        Value = value;
        TimeOfReading = timeOfReading;
    }
}