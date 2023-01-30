using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

using OpenDatabase;
using OpenDatabaseAPI;

namespace OpenDatabase.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PostGRESDatabase database = new PostGRESDatabase(DatabaseConfiguration.LoadFromFile(DatabaseConfiguration.DefaultDatabaseConfigFile));

            database.Connect();

            Record record;
            
            // Console.WriteLine((record = new Record(new string[]{"id", "val"}, new object[]{ Guid.NewGuid().ToString(), "value" })).ToString());
            // Console.WriteLine(database.InsertRecord(record, "test"));
            // Console.WriteLine(JsonConvert.SerializeObject(database.FetchQueryData($"SELECT * FROM test WHERE id='{record.Values[0].ToString()}';", "test")));
        }
    }
}

