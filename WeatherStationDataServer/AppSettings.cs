using Microsoft.Extensions.Configuration;

namespace WeatherStationDataServer;

public static class AppSettings
{
    public static IConfigurationRoot Configuration { get; }

    static AppSettings()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .SetBasePath(Directory.GetCurrentDirectory())
            .Build();
    }
}
