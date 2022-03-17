using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenDatabase
{
    public class Program
    {
        static void Main()
        {
			Database.Connect();

			Database.InsertRecord(new Record(new string[] { "col", "lol" }, new object[] { "va", 10 }), "test1");
        }
    }
}
