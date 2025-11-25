namespace WeatherStationDataServer.Models.Writers;

public enum LogLevel 
{
    INFO,
    WARNING,
    ERROR
}

public sealed class FileSaver : ITextWriter, ILogLevel
{
    private string _directoryPath;
    private string _filePath; 

    private FileSaver() 
    {
        _directoryPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Logs");

        _filePath = Path.Combine(_directoryPath, "Logs.txt");

        if (!Directory.Exists(_directoryPath)) 
            Directory.CreateDirectory(_directoryPath);
    }

    private static FileSaver? _instacne;
    public static FileSaver Instance 
    { 
        get => _instacne ??= new FileSaver(); 
    }

    private string LogLevelToString(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.INFO    => "INFO",
            LogLevel.WARNING => "WARNING",
            LogLevel.ERROR   => "ERROR",
            _ => throw new ArgumentException("Specified log level not found")
        };

    public string WriteData(string text) 
    {
        var convertedData = $"{DateTime.Now}\t{text}\n";
        File.AppendAllText(
            _filePath,
            convertedData);
        return convertedData;
    }

    /// <summary>
    /// Accepts LogLevel and returns write function 
    /// with argument modified to contain log level
    /// </summary>
    public Func<string, string> LogLevelWrite(LogLevel level) =>
        text => WriteData($"{LogLevelToString(level)}: " + text);
    
}