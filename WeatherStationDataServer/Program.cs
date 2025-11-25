using Geolocation;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using System.Diagnostics;
using WeatherStationDataServer.Converter;
using WeatherStationDataServer.Models;
using WeatherStationDataServer.Models.DataModel;
using WeatherStationDataServer.Models.Writers;
using WeatherStationDataServer.Receivers;

namespace WeatherStationDataServer;

internal class Program
{
    static async Task Main(string[] args)
    {
        GenericUnitConverter.AddUnits(new HashSet<Enum>
        {
            TemperatureUnits.C,
            TemperatureUnits.F,
            TemperatureUnits.K
        });

        GenericUnitConverter.AddUnits(new HashSet<Enum>
        {
            ParticalDensityUnits.PPB,
            ParticalDensityUnits.PPM
        });

        GenericUnitConverter.SetBaseUnit(TemperatureUnits.C);
        GenericUnitConverter.AddConverter(TemperatureUnits.K, v => v - 273.15, v => v + 273.15);
        GenericUnitConverter.AddConverter(TemperatureUnits.F, v => (v - 32.0) / 1.8, v => v * 1.8 + 32.0);

        GenericUnitConverter.SetBaseUnit(ParticalDensityUnits.PPM);
        GenericUnitConverter.AddConverter(ParticalDensityUnits.PPB, v => v / 1000.0, v => v * 1000.0);

        Debug.WriteLine("Converters have been set up");

        var device = WeatherDeviceBuilder.Instance
                        .DeviceParameters("SomeId", new Coordinate(50.31, 30.29))
                        .AddWeatherParameter("temperature", TemperatureUnits.C)
                        .AddWeatherParameter("humidity", RelativeHumidity.PROCENT)
                        .AddWeatherParameter("airquality", ParticalDensityUnits.PPB)
                        .AddWeatherParameter("rain", ADCUnit.TWELVE_BIT)
                        .AddWeatherParameter("noise", SoundVolumeUnits.DB)
                        .AddWeatherParameter("co", ParticalDensityUnits.PPM)
                        .AddWeatherParameter("sleepingtime", TimeUnit.Milliseconds)
                        .Build();

        var currentTime = DateTime.UtcNow;
        /*var device = new WeatherDevice(
            "SomeId", 
            new Coordinate(50.31, 30.29),
            new Dictionary<string, WeatherParameters>
            {
                {   "Temperature".ToLower(), 
                    new WeatherParameters(
                        TemperatureUnits.C,
                        new List<WeatherParameterValue>{ })},
                {   "Humidity".ToLower(),
                    new WeatherParameters(
                        RelativeHumidity.PROCENT,
                        new List<WeatherParameterValue>{ })},
                {   "AirQuality".ToLower(),
                    new WeatherParameters(
                        ParticalDensityUnits.PPB,
                        new List<WeatherParameterValue>{ })},
                {   "Rain".ToLower(),
                    new WeatherParameters(
                        ADCUnit.TWELVE_BIT,
                        new List<WeatherParameterValue>{ })},
                {   "Noise".ToLower(),
                    new WeatherParameters(
                        SoundVolumeUnits.DB,
                        new List<WeatherParameterValue>{ })},
                {   "CO".ToLower(),
                    new WeatherParameters(
                        ParticalDensityUnits.PPM,
                        new List<WeatherParameterValue>{ })},
                {   "SleepingTime".ToLower(),
                    new WeatherParameters(
                        TimeUnit.Milliseconds,
                        new List<WeatherParameterValue>{ })},
            });        */


        //IDataReceiver receiver = new HttpDataServer(FileSaver.Instance);
        IDataReceiver receiver = new MqttDataServer(
            new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 1883)
                .WithClientId("id")
                .Build(),
            new MqttClientFactory().CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => { f.WithTopic("test_sensor_data"); })
                .Build());


        receiver.RecieveEvent += (obj, recievedData) =>
        {
            var weatherDeviceData = WeatherDataConverter.Convert(recievedData.Body);
            // weatherDeviceData.DeviceId is used to select specific device in some collection of devices
            device.LastReceivalTime = weatherDeviceData.LastReceivalTime;

            foreach (var kv in weatherDeviceData.Parameters) 
            {
                device.Parameters
                      .GetValueOrDefault(kv.Key)
                      ?.Values
                      .Add(new WeatherParameterValue(
                          kv.Value, 
                          weatherDeviceData.LastReceivalTime));
            }

            //QuestDBConnection.Send(weatherDeviceData).Wait();

            device.ManageDataCount();

            FileSaver.Instance
                     .LogLevelWrite(LogLevel.INFO)
                     ($"Received Data from Client. " +
                     $"Next data in {weatherDeviceData.Parameters["sleepingtime"]} " +
                     $"{device.Parameters["sleepingtime"].Unit}");
            
            ConsoleWriter.Instance.WriteData(
                WeatherForecast.WeatherAnalysis(device));
            
            foreach (var kv in device.Parameters)
            {
                Console.WriteLine($"{kv.Key} - " +
                    $"{device.Parameters[kv.Key].Unit}");
                foreach (var val in device.Parameters[kv.Key].Values)
                {
                    Console.WriteLine($"  {val.TimeOfReading}: {val.Value}");
                }
            }
            Console.WriteLine();
        };

        receiver.Start();
    }
}
