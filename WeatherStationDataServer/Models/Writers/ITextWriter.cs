namespace WeatherStationDataServer.Models.Writers;


public interface ITextWriter
{
    string WriteData(string text);
}


public interface ILogLevel
{
    Func<string, string> LogLevelWrite(LogLevel level);
}