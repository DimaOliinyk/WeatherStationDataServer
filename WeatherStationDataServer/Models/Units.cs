namespace WeatherStationDataServer.Models;

public enum TemperatureUnits
{
    C,
    F,
    K
}
public enum ParticalDensityUnits
{
    PPM,
    PPB
}

public enum SoundVolumeUnits
{
    DB
}

public enum RelativeHumidity
{
    PROCENT
}

public enum ADCUnit
{
    TWELVE_BIT
}

public enum TimeUnit
{
    Seconds, 
    Milliseconds
}

public static class GenericUnitConverter
{
    private static HashSet<HashSet<Enum>> _setOfUnits = new HashSet<HashSet<Enum>>();
    public static bool AddUnits(HashSet<Enum> units) => _setOfUnits.Add(units);

    private static Dictionary<Enum, Func<double, double>> _ToBaseConverters = new();
    private static Dictionary<Enum, Func<double, double>> _fromBaseConverters = new();

    public static void AddConverter(Enum notBaseUnit,
                                    Func<double, double> toBaseConverter,
                                    Func<double, double> fromBaseConverter)
    {
        _ToBaseConverters.Add(notBaseUnit, toBaseConverter);
        _fromBaseConverters.Add(notBaseUnit, fromBaseConverter);
    }

    public static void SetBaseUnit(Enum baseUnit)
    {
        _ToBaseConverters.Add(baseUnit, v => v);
        _fromBaseConverters.Add(baseUnit, v => v);
    }

    public static double Convert(double value, Enum fromUnit, Enum toUnit) =>
        _setOfUnits.Any(x => x.Contains(fromUnit) && x.Contains(toUnit)) switch
        {
            true => _fromBaseConverters.GetValueOrDefault(toUnit)?.Invoke(
                       _ToBaseConverters.GetValueOrDefault(fromUnit)?.Invoke(value) ?? double.NaN)
                    ?? double.NaN,
            _ => double.NaN
        };
}