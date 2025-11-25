using MQTTnet;
using System.Diagnostics;
using System.Text;
using static WeatherStationDataServer.Receivers.HttpDataServer;

namespace WeatherStationDataServer.Receivers;


public class MqttWeatherArgs : WeatherArgs
{
    public MqttWeatherArgs(string topic, string body): base(body)
    {
        Topic = topic;
    }
    public string Topic { get; set; }
}


public class MqttDataServer : IDataReceiver, IDisposable
{
    public event RecieveEventHandler? RecieveEvent;

    private MqttClientOptions _mqttClientOptions;
    private MqttClientFactory _mqttFactory;
    private MqttClientSubscribeOptions _mqttSusbscribeOptions;
    private IMqttClient _mqttClient;

    public MqttDataServer(
        MqttClientOptions clientOptions, 
        MqttClientSubscribeOptions clientSubscribeOptions)
    {
        _mqttFactory = new MqttClientFactory();
        _mqttClientOptions = clientOptions;
        _mqttSusbscribeOptions = clientSubscribeOptions;

        _mqttClient = _mqttFactory.CreateMqttClient();

        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            RecieveEvent?.Invoke(this, new MqttWeatherArgs(
                e.ApplicationMessage.Topic,
                $"{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}"));

            Debug.WriteLine($"Message received {e.ApplicationMessage.Topic}: " +
                $"{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");

            return Task.CompletedTask;
        };
    }

    // TODO: add proper async support
    public async void Start()
    {
        var connResult = _mqttClient.ConnectAsync(
            _mqttClientOptions, 
            CancellationToken.None).GetAwaiter().GetResult();

        if (connResult.ResultCode == MqttClientConnectResultCode.Success)
        {
            Debug.WriteLine("Successful MQTT connection");
            _mqttClient.SubscribeAsync(_mqttSusbscribeOptions, CancellationToken.None).Wait();

            while (true)
            {
                //await Task.Delay(1000).ConfigureAwait(false);
                Thread.Sleep(1000);
            }
        }
    }

    public void Dispose() => _mqttClient.Dispose();
}

/*
                    MQTT documentation
 1. https://dev.to/emqx/how-to-use-mqtt-in-c-with-mqttnet-1j97
 2. https://www.hivemq.com/blog/how-to-get-started-with-mqtt/
 */
