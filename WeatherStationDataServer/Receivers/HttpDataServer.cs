using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WeatherStationDataServer.Models.Writers;

namespace WeatherStationDataServer.Receivers;

public class WeatherArgs : EventArgs
{
    public WeatherArgs(string body)
    {
        Body = body;    
    }
    public string Body { get; set; }
}


public class HttpDataServer : IDisposable, IDataReceiver
{
    private TcpListener _listener;
    private int _port = 8181;
    private IPAddress _ip = IPAddress.Parse("127.0.0.1");
    ILogLevel _logger;

    public delegate void RecieveEventHandler(object s, WeatherArgs e);
    public event RecieveEventHandler? RecieveEvent;

    public HttpDataServer(ILogLevel logger)
    {
        _listener = new TcpListener(_ip, _port);
        _logger = logger;
    }

    public void Start() 
    {
        _listener.Start();
        _logger.LogLevelWrite(LogLevel.INFO)($"Starting server {_ip}:{_port}");

        while (true)
        { 
            TcpClient client = _listener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();

            byte[] requestBytes = new byte[1024];
            int bytesRead = stream.Read(requestBytes, 0, requestBytes.Length);

            string body = string.Empty;
            try
            {
                string request = Encoding.UTF8.GetString(requestBytes, 0, bytesRead);
                body = request.Split('{')[1]
                                  .Replace('}', ' ');
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.WriteLine($"Problem with json object in httpbody: {ex.Message}");
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Something went wrong when processing the post request: {ex.Message}");
            }
            Debug.WriteLine(body);

            RecieveEvent?.Invoke(this, new WeatherArgs(body));
            client.Close();
        }
    }

    public void Dispose() => _listener.Stop();
}
