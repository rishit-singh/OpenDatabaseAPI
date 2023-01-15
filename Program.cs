using System;
using System.Collections.Generic;
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
            Console.WriteLine(JsonConvert.SerializeObject(database.FetchQueryData("SELECT * FROM test;")));
            database.Disconnect();
            
        }
    }
}
