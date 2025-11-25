namespace WeatherStationDataServer.Models;

interface ICacheManageable 
{
    public long DataCount { get; set; }
    public void ManageDataCount();
}
