using Geolocation;
using WeatherStationDataServer.Models;
using WeatherStationDataServer.Models.DataModel;

namespace UnitTests;

public class WeatherDeviceUnitTest
{

    private WeatherDevice _weatherDevice;

    [SetUp]
    public void Setup()
    {
        var currentTime = DateTime.UtcNow;
        _weatherDevice = new WeatherDevice(
            "SomeId",
            new Coordinate(50.31, 30.29),
            new Dictionary<string, WeatherParameters>
            {
                {   "Temperature".ToLower(),
                    new WeatherParameters(
                        TemperatureUnits.C,
                        new List<WeatherParameterValue>{ })}
            });
    }

    [Test]
    public void Removes_Values_When_CountExceedes() 
    {
        _weatherDevice.DataCount = 3;
        foreach(var i in Enumerable.Range(1, 4))
            _weatherDevice.Parameters.GetValueOrDefault("temperature")!.Values.Add(
                new WeatherParameterValue(i, DateTime.UtcNow));
        
        _weatherDevice.ManageDataCount();
        
        // Correct count
        Assert.That(_weatherDevice.Parameters.GetValueOrDefault("temperature")!.Values.Count == 3); 
        // Correct order
        Assert.That(_weatherDevice.Parameters.GetValueOrDefault("temperature")!.Values[0].Value == 2.0);    
        Assert.That(_weatherDevice.Parameters.GetValueOrDefault("temperature")!.Values[1].Value == 3.0);
        Assert.That(_weatherDevice.Parameters.GetValueOrDefault("temperature")!.Values[2].Value == 4.0);
    }

    [Test]
    public void Keeps_All_Values_When_Count_Is_Smaller_Then_Specifeid()
    {
        _weatherDevice.DataCount = 5;
        foreach (var i in Enumerable.Range(1, 4))
            _weatherDevice.Parameters.GetValueOrDefault("temperature")!.Values.Add(
                new WeatherParameterValue(i, DateTime.UtcNow));

        _weatherDevice.ManageDataCount();

        Assert.That(_weatherDevice.Parameters.GetValueOrDefault("temperature")!.Values.Count == 4);
    }

    [Test]
    public void Throws_If_Count_Set_To_Zero()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => _weatherDevice.DataCount = 0);
        TestContext.Out.WriteLine(ex.Message);
    }
}
