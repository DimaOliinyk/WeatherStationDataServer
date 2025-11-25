using WeatherStationDataServer.Converter;

namespace UnitTests;

public class WeatherDataConverterUnitTest
{
    private string[] _correctAttributeSet = [
        "temperature", "humidity", "pressure", 
        "rain", "airquality", "co", "sleepingtime"
    ];

    private double[] _correctAttributeValues = [
        28.77, 53.96, 996.44, 4095, 10, 470, 3000
    ];



    [Test]
    [TestCase("""
        {
            "deviceid": "Device1",
            "time": "26-10-2025 14_30_00",
            "temperature": 28.77,
            "humidity": 53.96,
            "pressure": 996.44,
            "rain": 4095,
            "airquality": 10,
            "co": 470,
            "sleepingtime": 3000
        }        
        """)]
    [TestCase ("{ \"deviceid\": \"Device1\", \"time\": \"26-10-2025 14_30_00\", \"temperature\": 28.77, \"humidity\": 53.96, \"pressure\": 996.44, \"rain\": 4095, \"airquality\": 10, \"co\": 470, \"sleepingtime\": 3000 }")]
    public void Conversion_Is_Correct(string correctJsonString) 
    {
        var data = WeatherDataConverter.Convert(correctJsonString);
        Assert.That(data.LastReceivalTime, Is.EqualTo(DateTime.Parse("26-10-2025 14:30:00")));
        Assert.That(data.DeviceId, Is.EqualTo("Device1"));

        foreach (var attr in _correctAttributeSet) 
        {
            Assert.That(data.Parameters.Keys.Contains(attr));
        }
        Assert.That(data.Parameters.Keys.Count == _correctAttributeSet.Length);

        for(int i = 0; i < _correctAttributeValues.Length; i++)
        {
            Assert.That(data.Parameters[_correctAttributeSet[i]] ==
                _correctAttributeValues[i]);
        }

    }


    [Test]
    public void Throws_If_Null_Or_WhiteSpace() 
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            WeatherDataConverter.Convert(String.Empty));
        TestContext.Out.WriteLine(ex.Message);

    }
}
