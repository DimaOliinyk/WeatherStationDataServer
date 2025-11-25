using WeatherStationDataServer.Models;
using WeatherStationDataServer.Models.DataModel;

namespace UnitTests;

public class WeatherDeviceBuilderUnitTest
{
    [Test]
    public void Created_Correctly() 
    {
        var device = WeatherDeviceBuilder
                            .Instance
                            .DeviceParameters("Device1", new Geolocation.Coordinate(50.31, 30.29))
                            .AddWeatherParameter("temperature", TemperatureUnits.C)
                            .AddWeatherParameter("humidity", RelativeHumidity.PROCENT)
                            .Build();

        Assert.That(device.DeviceId, Is.EqualTo("Device1"));
        Assert.That(device.Location, Is.EqualTo(new Geolocation.Coordinate(50.31, 30.29)));
        
        Assert.That(device.Parameters.ContainsKey("temperature"));
        Assert.That(device.Parameters.ContainsKey("humidity"));

        Assert.That(device.Parameters["temperature"].Unit, Is.EqualTo(TemperatureUnits.C));
        Assert.That(device.Parameters["humidity"].Unit, Is.EqualTo(RelativeHumidity.PROCENT));
    }

    [Test]
    public void Throws_When_Not_All_Parameters_Are_Set()
    {
        var ex = Assert.Throws<ArgumentException>(() => 
                    WeatherDeviceBuilder
                        .Instance
                        .AddWeatherParameter("temperature", TemperatureUnits.C)
                        .AddWeatherParameter("humidity", RelativeHumidity.PROCENT)
                        .Build());

        TestContext.Out.WriteLine(ex.Message);
    }
}
