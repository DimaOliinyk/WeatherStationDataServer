using System.Globalization;
using WeatherStationDataServer.Models.ReceiveDataModel;

namespace WeatherStationDataServer.Converter;


public static class WeatherDataConverter
{
    public static ReceiveWeatherDeviceData Convert(string dataToConvert)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataToConvert);

        dataToConvert = dataToConvert.Replace("{", null);
        dataToConvert = dataToConvert.Replace("}", null);

        var dataChunks =
                dataToConvert.Replace("\"", null)
                             .Trim(['\n', ' '])
                             .Split(",")
                             .Select(x => x.Trim())
                             .Select(x =>
                                (x.Split(':')[0].Trim(),
                                 x.Split(':')[1].Trim()));

        DateTime readTime = dataChunks.Where(x => x.Item1 == "time")
                                      .Select(x => DateTime.Parse(
                                          x.Item2.Replace('_', ':')))
                                      .First();

        string name = dataChunks.Where(x => x.Item1 == "deviceid")
                                .Select(x => x.Item2)
                                .First();
                
        Dictionary<string, double> weatherParametersValues = new();
        foreach (var chunk in dataChunks)
        {
            if (chunk.Item1 switch 
            {
                "time" or "deviceid" => true,
                _ => false
            }) continue;

            weatherParametersValues.Add(
                chunk.Item1.ToLower(),
                Double.Parse(chunk.Item2, NumberStyles.Any, CultureInfo.InvariantCulture));
        }
        return new ReceiveWeatherDeviceData(name, readTime, weatherParametersValues);
    }
}