namespace WeatherStationDataServer.Models.Writers
{
    public sealed class ConsoleWriter : ITextWriter
    {
        private ConsoleWriter() { }

        private static ConsoleWriter? _instance; 
        public static ConsoleWriter Instance 
        {
            get => _instance ??= new ConsoleWriter();
        }

        public string WriteData(string dataToWrite)
        {
            Console.WriteLine(dataToWrite);
            return dataToWrite;
        }
    }
}