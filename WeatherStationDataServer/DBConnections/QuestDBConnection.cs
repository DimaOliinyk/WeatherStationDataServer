using Microsoft.Extensions.Configuration;
using QuestDB;
using WeatherStationDataServer.Models.ReceiveDataModel;

namespace WeatherStationDataServer.DBConnections;

/*
CREATE TABLE IF NOT EXISTS Device1 (
  timestamp TIMESTAMP,
  temperature double,
  humidity double,
  pressure double,
  rain double,
  airquality double,
  co double,
  sleepingtime double
) TIMESTAMP(timestamp) PARTITION BY DAY WAL;

--DROP TABLE temperatures;

INSERT INTO temperatures
VALUES(
    '2021-10-05T11:31:35.878Z',
    28.0);

TRUNCATE TABLE Device1;
SELECT * FROM Device1;


 */

public class QuestDBConnection
{
    //private string _addr = "localhost:9000";
    //private string _login = "admin";
    //private string _password = "quest";
    
    private readonly QuestDB.Senders.ISender _sender;
    private static QuestDBConnection? _instance;

    private QuestDBConnection()
    {
        //_sender = Sender.New($"http::addr={_addr};username={_login};password={_password};");
        _sender = Sender.New(
            AppSettings.Configuration.GetConnectionString("QuestDBConnection")!);
    }

    private static QuestDBConnection _Instance 
    { 
        get => _instance ??= new QuestDBConnection();
    }

    public static async Task Send(ReceiveWeatherDeviceData data) 
    {
        var query = _Instance._sender.Table(data.DeviceId);
        
        foreach (var kv in data.Parameters)
        {
            // TODO: Segregate table ???
            query = BuildColumnValues(query, kv.Key, kv.Value);
        }
        query.At(data.LastReceivalTime);
        await _Instance._sender.SendAsync();
    }

    private static QuestDB.Senders.ISender BuildColumnValues(
        QuestDB.Senders.ISender kv, 
        string columnToAdd, 
        double valueToAdd) 
    {
        return kv.Column(columnToAdd, valueToAdd);
    }
}

/*
            Best Time series DB practices

Squeezing data:
+ Simple queries
+ Ease of changing scheme
+ Easy integration with ORMs
- Deletion

Splitting data:
+ Querying
+ Deletion
- Table naming
- Migration

Narrow layout:
+ Easy
- A lot of Join and Unioin
- Can result in many tables

Wide layout:
+ Quicker queries
- Adding more metric columns 
(also Nullable values)

Medium layout:
    The medium table above sits in between the narrow and wide layouts. 
    It has columns representing different data types or categories of 
    metrics but retains some level of generic structure.

Write-ahead logging (WAL) is a family of techniques for providing atomicity and durability in database systems.
A write ahead log is an append-only auxiliary disk-resident structure used for crash and transaction recovery. 
The changes are first recorded in the log, which must be written to stable storage, before the changes are 
written to the database.

Sources:
1. https://www.tigerdata.com/learn/best-practices-time-series-data-modeling-single-or-multiple-partitioned-tables-aka-hypertables
2. https://www.tigerdata.com/learn/designing-your-database-schema-wide-vs-narrow-postgres-tables


            Maintaining DB

My Method of Maintaining data:
1) Partition by month
2) After month passes -> group by hour and get average value
3) Then export gotten values to temp csv file 
4) Upload temp file to ftp server
5) Delete extra partitions if the amount is more than 3 years


Sources:
1. https://questdb.com/docs/operations/data-retention/
2. https://questdb.com/docs/operations/task-automation/
3. https://questdb.com/docs/reference/sql/overview/#postgresql
 */