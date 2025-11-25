using WeatherStationDataServer.Models;
using WeatherStationDataServer.Models.DataModel;

namespace WeatherStationDataServer;

public static class WeatherForecast
{
    public static string WeatherAnalysis(WeatherDevice weatherData)
    {
        var result = string.Empty;

        var temp = weatherData.Parameters
                              .GetValueOrDefault("Temperature".ToLower())
                              ?.Values
                              .Last()
                              .Value;
        var humid = weatherData.Parameters
                               .GetValueOrDefault("Humidity".ToLower())
                               ?.Values
                               .Last()
                               .Value;

        if (temp is null || humid is null)  return result;

        var tempC = GenericUnitConverter.Convert(
            (double)temp!,
            weatherData.Parameters
                       .GetValueOrDefault("Temperature".ToLower())!.Unit, 
            TemperatureUnits.C);

        if (temp >= 27.0)
        {
            result += $"Heat Index is {HeatIndex((double)temp, (double)humid)}.\n";
            result += $"Dew Point is {DewPoint((double)temp, (double)humid)}.\n";
        }

        return result;
    }


    /// <summary>
    /// Approximated Heat Index for temperatures >= 27C
    /// </summary>
    /// <param name="temperature"></param>
    /// <param name="relativeHumidity"></param>
    /// <returns></returns>
    public static double HeatIndex(double temperature, double relativeHumidity) =>
        -8.784_694_755_56
        + 1.611_394_11 * temperature
        + 2.338_548_838_89 * relativeHumidity
        - 0.146_116_05 * temperature * relativeHumidity
        - 0.012_308_094 * temperature * temperature
        - 0.016_424_827_777_8 * relativeHumidity * relativeHumidity
        + (2.211_732 / 1000.0) * temperature * temperature * relativeHumidity
        + (7.2546 / 10_000.0) * temperature * relativeHumidity * relativeHumidity
        - (3.582 / 1_000_000.0) * temperature * temperature * relativeHumidity * relativeHumidity;

    public static double DewPoint(double temperature, double relativeHumidity) =>
        temperature - (100.0 - relativeHumidity) / 5.0;
}
