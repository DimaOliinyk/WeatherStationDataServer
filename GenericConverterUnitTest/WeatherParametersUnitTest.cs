using WeatherStationDataServer.Models;
using WeatherStationDataServer.Models.DataModel;

namespace UnitTests;

public class WeatherParametersUnitTest
{
    private WeatherParameters _weatherParameters;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        GenericUnitConverter.AddUnits(new HashSet<Enum>
        {
            TemperatureUnits.C,
            TemperatureUnits.F,
            TemperatureUnits.K
        });

        GenericUnitConverter.SetBaseUnit(TemperatureUnits.C);
        GenericUnitConverter.AddConverter(TemperatureUnits.K, v => v - 273.15, v => v + 273.15);
        GenericUnitConverter.AddConverter(TemperatureUnits.F, v => (v - 32.0) / 1.8, v => v * 1.8 + 32.0);

        GenericUnitConverter.AddUnits(new HashSet<Enum>
        {
            ParticalDensityUnits.PPB,
            ParticalDensityUnits.PPM
        });

        GenericUnitConverter.SetBaseUnit(ParticalDensityUnits.PPM);
        GenericUnitConverter.AddConverter(ParticalDensityUnits.PPB, v => v / 1000.0, v => v * 1000.0);
    }

    [SetUp]
    public void Setup()
    {
        _weatherParameters = new WeatherParameters(
            TemperatureUnits.C, 
            new List<WeatherParameterValue> 
            { 
                new WeatherParameterValue(25, DateTime.UtcNow),
                new WeatherParameterValue(35, DateTime.UtcNow),
                new WeatherParameterValue(0, DateTime.UtcNow),
            });
    }

    [Test]
    public void Changing_Unit_Property_Converts_All_Values() 
    {
        var previousValues = _weatherParameters.Values;
        _weatherParameters.Unit = TemperatureUnits.K;       // Changing units


        for(int i = 0; i < _weatherParameters.Values.Count; i++) 
        {
            TestContext.Out.WriteLine($"{_weatherParameters.Values[i].Value} == {previousValues[i].Value} + 273.15");
            Assert.That(_weatherParameters.Values[i].Value == previousValues[i].Value + 273.15);

            Assert.That(_weatherParameters.Values[i].TimeOfReading == 
                previousValues[i].TimeOfReading);   // Time property is unaltered
        }

    }

    [Test]
    public void Conversion_To_Units_Of_Different_Physical_Quantities_Throws()
    {
        // Change units to incompatible one
        Assert.Throws<ArgumentException>(() => _weatherParameters.Unit = ParticalDensityUnits.PPM);
    }
}
