using WeatherStationDataServer.Models;

namespace UnitTests;

public class GenericConverterUnitTests
{
    [OneTimeSetUp]
    public void Setup()
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

    [Test]
    public void Conversion_Between_Same_Units_Returns_Same_Result()
    {
        var result = GenericUnitConverter.Convert(22.0, TemperatureUnits.C, TemperatureUnits.C);
        Assert.That(result, Is.EqualTo(22.0));
    }

    [Test]
    public void Simple_Conversions_Work()
    {
        var result = GenericUnitConverter.Convert(15.0, TemperatureUnits.C, TemperatureUnits.K);
        Assert.That(result, Is.EqualTo(288.15));

        result = GenericUnitConverter.Convert(288.15, TemperatureUnits.K, TemperatureUnits.C);
        Assert.That(result, Is.EqualTo(15.0));
    }
       

    [Test]
    public void Adding_Same_Converter_Throws()
    {        
        var ex = Assert.Throws<ArgumentException>(() => GenericUnitConverter.AddConverter(
            TemperatureUnits.K, 
            v => v - 273.15, 
            v => v + 273.15));
        TestContext.Out.WriteLine($"Exception message: {ex.Message}");
    }

    [Test]
    public void Different_Units_Produce_NaN()
    {
        Assert.That(double.IsNaN(GenericUnitConverter.Convert(
            42.0, 
            TemperatureUnits.C, 
            ParticalDensityUnits.PPM)));
    }   
}
