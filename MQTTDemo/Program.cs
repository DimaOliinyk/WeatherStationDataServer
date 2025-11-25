using MQTTnet;
using System.Text;

namespace MQTTDemo;
/*
 * Read up on
 https://dev.to/emqx/how-to-use-mqtt-in-c-with-mqttnet-1j97

 https://www.hivemq.com/blog/how-to-get-started-with-mqtt/
 */

public class Program
{
    static async Task Main(string[] args)
    {
        var mqttFactory = new MqttClientFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1", 1883)
            .WithClientId("id")
            .Build();

        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            Console.WriteLine($"Message received {e.ApplicationMessage.Topic}: " +
                $"{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            return Task.CompletedTask;
        };

        var mqttSusbscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => { f.WithTopic("test_sensor_data"); })
            .Build();

        var connResult = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        if(connResult.ResultCode == MqttClientConnectResultCode.Success)
        {
            Console.WriteLine("Success");
            await mqttClient.SubscribeAsync(mqttSusbscribeOptions, CancellationToken.None);
            while (true) 
            {
                await Task.Delay(1000);
            }
        }
    }
}
