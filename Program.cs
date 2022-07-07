using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

using OpenDatabase;

namespace OpenDatabase.Test
{
    public class Program
    {
        public static void Main()
        {

            new Thread(() => {
                Database db = new Database();
                Console.WriteLine(JsonConvert.SerializeObject(db.FetchQueryData("SELECT * FROM users;")));
            }).Start();

            new Thread(() => {
                Database db = new Database();

                Console.WriteLine(JsonConvert.SerializeObject(db.FetchQueryData("SELECT * FROM users;")));
            }).Start();
        }
    }
}
