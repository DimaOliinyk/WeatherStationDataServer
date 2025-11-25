namespace WeatherStationDataServer.Models.DataModel;



/// <summary>
/// Represents a list of values of one single weather 
/// parameter and its unit (e.g. Humidity-% or Temperature-C)
/// </summary>
public class WeatherParameters
{
    private Enum _unit;
    public Enum Unit 
    { 
        get => _unit;
        set 
        {
            if (_unit == value) 
                return;
            var tempList = new List<WeatherParameterValue>();

            foreach (var weatherValue in Values)
            {
                var convertedValue = GenericUnitConverter.Convert(weatherValue.Value, _unit, value);
                if (double.IsNaN(convertedValue)) 
                    throw new ArgumentException("Could not convert to specified units");
                tempList.Add(new WeatherParameterValue(convertedValue, weatherValue.TimeOfReading));
            }
            Values = tempList;
            _unit = value;   
        }
    } 
    public List<WeatherParameterValue> Values { get; set; }

    public WeatherParameters(Enum unit, List<WeatherParameterValue> values)
    {
        Values = values;
        _unit = unit;
    }
}
