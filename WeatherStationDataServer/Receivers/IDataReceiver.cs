using static WeatherStationDataServer.Receivers.HttpDataServer;

namespace WeatherStationDataServer.Receivers;

public interface IDataReceiver
{
    public event RecieveEventHandler RecieveEvent;
    
    public void Start();
}