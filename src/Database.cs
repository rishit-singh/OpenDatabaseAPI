using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using OpenDatabase.Logs;
using OpenDatabase.Json;

namespace OpenDatabase
{
	///<summary>
	///	Stores the Database configuration to be used by the SQL server.
	///</summary>
	public struct DatabaseConfiguration
	{
		public string HostName;	//	Name of the database host server

		public string DatabaseName;	//	Name of the DB to connect to.

		public string UserID;	//	Database user's ID.

		public string Password;	//	Database users's password.

		public bool IntegratedSecurity;		//	Integrated security toggle

		public readonly string ConnectionString { get { return this.GetConnectionString(); } }	//	SQL connection string.

		public string GetConnectionString()
		{	
			string integratedSecurity = (this.IntegratedSecurity) ? "True" : "False";

			return $"Server={this.HostName};Database={this.DatabaseName};User Id={this.UserID};Password={this.Password}";
		}

		public DatabaseConfiguration(string hostName, string databaseName, string userID, string password)
		{
			this.UserID = userID;
			this.Password = password;
			this.HostName = hostName;
			this.DatabaseName = databaseName;
			this.IntegratedSecurity = true;
		}

		public DatabaseConfiguration(string hostName, string databaseName, string userID, string password, bool integratedSecurity)
		{
			this.UserID = userID;
			this.Password = password;
			this.HostName = hostName;
			this.DatabaseName = databaseName;
			this.IntegratedSecurity = integratedSecurity;
		}

		public DatabaseConfiguration(string configFilePath)
		{
			Hashtable configHash = Json.Json.GetJsonHashtable(configFilePath);

			try
			{
				this.UserID = configHash["UserID"].ToString();
				this.Password = configHash["Password"].ToString();
				this.HostName = configHash["HostName"].ToString();
				this.DatabaseName = configHash["DatabaseName"].ToString();
				this.IntegratedSecurity = (configHash["IntegratedSecurity"].ToString() == "True") ? true : false;
			}
			catch (KeyNotFoundException)
			{
				Logger.Log($"Invalid configuration provided.");

				this.UserID = null;
				this.Password = null;
				this.HostName = null;
				this.DatabaseName = null;
				this.IntegratedSecurity = true;
			}
		}

		public DatabaseConfiguration(Hashtable configHash)
		{
			this.UserID = configHash["UserID"].ToString();
			this.Password = configHash["Password"].ToString();
			this.HostName = configHash["HostName"].ToString();
			this.DatabaseName = configHash["DatabaseName"].ToString();
			this.IntegratedSecurity = (configHash["IntegratedSecurity"].ToString() == "True") ? true : false;
		}
	}


	public struct ComparisionPair<T>
	{	
		T Left,
	  		Right;	 

		public string GetComparisionString(string operators)
		{
			return $"{Convert.ToString(this.Left)}{operators}{Convert.ToString(this.Right)}";
		}

		public ComparisionPair(T left, T right)	
		{
			this.Left = left;
			this.Right = right;
		}
	}

	/// <summary>
	/// Stores a database record's keys and values. 
	/// </summary>
	public struct Record
	{
		public string[] Keys;
		
		public object[] Values;

		public Record(string[] keys, object[] values)
		{
			this.Keys = keys;
			this.Values = values; 
		}
	}

	public class QueryBuilder
	{
		public enum Command
		{
			Insert,
			Update,
			Alter,
			Drop
		}

		public enum SubCommand
		{
			Set,
			Where		
		}

		public static string[] CommandStrings = new string[] {
			"INSERT",
			"UPDATE",
			"ALTER",
			"DROP"
		};

		public static string[] SubCommandStrings = new string[] {
			"SET",
			"WHERE"
		};

		public static string GetValueString<T>(T value)
		{
			string valueString = null;
	
			Type valueType = value.GetType();
			
			 if (valueType == typeof(int))
				valueString = Convert.ToString(value);
			 else if (valueType == typeof(char))
				valueString = $"\'{Convert.ToString(value)}\'";
			 
			 else if (valueType == typeof(string))
				valueString = $"\'{Convert.ToString(value)}\'";
			 
			 else if (valueType == typeof(bool))
				valueString = (value.Equals(true)) ? "TRUE" : "FALSE";
			 
			 else;

			 return valueString;
		}

		public static string GetValueFunctionString(Hashtable data)
		{
			int size = data.Keys.Count;
	
			string valueFunctionString = "VALUES(";
		
			string[] keys = new string[size];

			data.Keys.CopyTo(keys, 0);

			for (int x = 0; x < size; x++)
				if (x == (size - 1))
					valueFunctionString += $"{QueryBuilder.GetValueString(data[keys[x]])})";
				else
					valueFunctionString += $"{QueryBuilder.GetValueString(data[keys[x]])}, ";

			return valueFunctionString;
		}

		public static string GetValueFunctionString(Record data)
		{
			int size = data.Values.Length;
	
			string valueFunctionString = "VALUES(";

			for (int x = 0; x < size; x++)
				if (x == (size - 1))
					valueFunctionString += $"{QueryBuilder.GetValueString(data.Values[x])})";
				else
					valueFunctionString += $"{QueryBuilder.GetValueString(data.Values[x])}, ";

			return valueFunctionString;
		}
		
		/// <summary>
		///	Gets the SQL SET function string. 
		/// </summary>
		/// <param name="record"> Record instance </param>
		/// <returns></returns>
		public static string GetSetString(Record record)
		{
			string setString = "SET ";
			
			int size = record.Keys.Length;
	
			for (int x = 0; x < size; x++)
				if (x == (size - 1))
					setString += $"{record.Keys[x]}={QueryBuilder.GetValueString(record.Values[x])}";
				else
					setString += $"{record.Keys[x]}={QueryBuilder.GetValueString(record.Values[x])}, ";

			return setString;
		}

		public static string GetInsertQuery(string tableName, Hashtable data)
		{
			string queryString = null;

			queryString  = $"{QueryBuilder.CommandStrings[(int)QueryBuilder.Command.Insert]} INTO {tableName} {QueryBuilder.GetValueFunctionString(data)};";

			return queryString;
		}

		public static string GetInsertQuery(string tableName, Record data)
		{
			return $"{QueryBuilder.CommandStrings[(int)QueryBuilder.Command.Insert]} INTO {tableName} {QueryBuilder.GetValueFunctionString(data)};";
		}

		public static string GetUpdateQuery(string tableName, Record data)
		{
			return $"UPDATE {tableName} {QueryBuilder.GetSetString(data)}";
		}
	}

	/// <summary>
	/// Handles database connectivity, data fetching and editing.		
	/// </summary>
	public class Database	
	{
		public static string DefaultDatabaseConfigurationFile = "DatabaseConfig.json";

		public static DatabaseConfiguration DefaultDatabaseConfiguration = new DatabaseConfiguration(Database.DefaultDatabaseConfigurationFile);	//	DB config based on the provided config files
		public static SqlConnection DefaultSqlConnection;	//	Global default SqlConnection based on DefaultDatabaseConfiguration
	

		/// <summary>
		/// Creates a connection to the specified database.
		/// </summary>
		/// <param name="dbname"> database name </param>
		/// <returns> SqlConnection instance storing the connection value. </returns>
		public static SqlConnection Connect()
		{
			try
			{
				Database.DefaultSqlConnection = new SqlConnection(Database.DefaultDatabaseConfiguration.ConnectionString);
			}
			catch (Exception e)
			{
				Logger.Log($"SQL Sever connection error: {e.Message}.");
			}
		
			return Database.DefaultSqlConnection;			
		}

		///	<summary>
		/// Excecutes a query in the DefaultSqlConnection	
		///	</summary>
		///	<param name="query"> Query string. </param>
		///	<returns> Execution state bool. </returns>
		public static bool ExecuteQuery(string query)
		{
			SqlCommand command = new SqlCommand(query, Database.DefaultSqlConnection);
			
			Database.DefaultSqlConnection.Open();
			
			command.ExecuteNonQuery();

			Database.DefaultSqlConnection.Close();

			return true;
		}
	
		/// <summary>
		///	Gets the field names from a data record instance.
		/// </summary>
		/// <param name="record"> IDataRecord instance. </param>
		/// <returns> Array of fieldnames. </returns>
		private static string[] GetFieldNames(IDataRecord record)		
		{
			Stack<string> fieldStack = new Stack<string>();
			
			int size = record.FieldCount;

			for (int x = (size - 1); x >= 0; x--)
				fieldStack.Push(record.GetName(x));	

			return fieldStack.ToArray();
		}

		/// <summary>
		///	Gets the field names from a data record instance.
		/// </summary>
		/// <param name="record"> IDataRecord instance. </param>
		/// <returns> Array of fieldnames. </returns>
		private static object[] GetValueNames(IDataRecord record)		
		{
			Stack<object> fieldStack = new Stack<object>();
			
			int size = record.FieldCount;

			for (int x = (size - 1); x >= 0; x--)
				fieldStack.Push(record[x]);	

			return fieldStack.ToArray();
		}

		///	<summary>
		///	Fetches the data returned by executing the provided query.	
		///	</summary>
		///	<param name="query"> SQL Query. </param>
		///	<returns> Hashtable containing the fetched data. </returns>
		public static Record[] FetchQueryData(string query)
		{
			Stack<string> Fields = new Stack<string>();
			
			Stack<object> Data = new Stack<object>();

			Stack<Record> records = new Stack<Record>();

			SqlCommand command = null;

			SqlDataReader dataReader = null;
				
			command = new SqlCommand(query, Database.DefaultSqlConnection);

			Database.DefaultSqlConnection.Open();			
			
			dataReader = command.ExecuteReader();
			
			while (dataReader.Read())
				records.Push(new Record(Database.GetFieldNames(dataReader), Database.GetValueNames(dataReader)));
			
			dataReader.Close();

			return records.ToArray();
		}

		/// <summary>
		///	Inserts the provided record into provided table.
		/// </summary>
		/// <param name="recordHashtable"> Hashtable storing the record value. </param>
		/// <returns> Execution state. </returns>
		public static bool InsertRecord(Hashtable recordHashtable, string tableName)
		{
			Database.ExecuteQuery(QueryBuilder.GetInsertQuery(tableName, recordHashtable));

			return true;
		}

		/// <summary>
		///	Inserts the provided record into provided table.
		/// </summary>
		/// <param name="record"> Record to be inserted </param>
		/// <returns> Execution state. </returns>
		public static bool InsertRecord(Record record, string tableName)
		{
			try
			{
				Database.ExecuteQuery(QueryBuilder.GetInsertQuery(tableName, record));
			}
			catch (SqlException)
			{
				Logger.Log("Insertion error occured, the provided record could be a duplicate.");
				
				return false;	
			}

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		/// <param name=""></param>
		/// <param name=""></param>
		public static bool UpdateRecord(string ID, Record record, string tableName)
		{
			string query = $"UPDATE {tableName} {QueryBuilder.GetSetString(record)} WHERE ID='{ID}';";

			Logger.ConsoleLog(query);

			Database.Connect();
			Database.ExecuteQuery(query);
			
			return true;	
		}
			
		public static int GetFieldCount(string tableName)
		{
			return Convert.ToInt32(Database.FetchQueryData($"SELECT COUNT(*) FROM {tableName};")[0].Values[0]);
		} 
	}
}
