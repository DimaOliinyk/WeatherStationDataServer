using QuestDB;

namespace QuestDbTest;

internal class Program
{
    static async Task Main(string[] args)
    {
        using var sender = Sender.New("http::addr=localhost:9000;username=admin;password=quest;");
        await sender.Table("temperatures")
            .Column("temperature", 20.0)
            .AtAsync(DateTime.UtcNow);
        await sender.SendAsync();
    }
}
