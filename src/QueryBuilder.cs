using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace OpenDatabase
{
	public enum Operator
	{
		Less,
		More,
		Equal,
		LessEqual,
		MoreEqual
	}

	public class ComparisonPair<T>
	{
		protected string[] OperatorChars = new string[] {
			"<",
			">",
			"=",
			"<=",
			">"	
		};
		
		T Left,
	  		Right;

		public Operator ComparisionOperator;
		
		protected char OperatorChar; 
		
		public string ToString(string operators)
		{
			return $"{Convert.ToString(this.Left)}{operators}{Convert.ToString(this.Right)}";
		}

		public ComparisonPair(T left, T right, Operator comparisonOp)
		{
			this.ComparisionOperator = comparisonOp;
			this.Left = left;
			this.Right = right;
		}
	}

	public class Condition<T>
	{
		public List<ComparisonPair<T>>[] Comparisons;

		public Condition<T> And(ComparisonPair<T> comparisonPair)
		{
			this.Comparisons[0].Add(comparisonPair);
			
			return this;
		}

		public Condition<T> Or(ComparisonPair<T> comparisonPair)
		{
			this.Comparisons[1].Add(comparisonPair);
			
			return this;
		}

		public override string ToString()
		{
			string conditionString = null;
			
			for (int x = 0; x < this.Comparisons[0].Count; x++)
				conditionString += $"{this.Comparisons[0][x].ToString()} AND";
		
			conditionString += $"{this.Comparisons[0][this.Comparisons[0].Count - 1].ToString()} ";
			
			for (int x = 0; x < this.Comparisons[1].Count; x++)
				conditionString += $"{this.Comparisons[1][x].ToString()} OR";
		
			conditionString += $"{this.Comparisons[1][this.Comparisons[1].Count - 1].ToString()} ";
			return conditionString;
		}

		public Condition()
		{
			this.Comparisons = new List<ComparisonPair<T>>[2];
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

		public static string[] SubCommandStrings = new string[]
		{
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

		public static string GetRecordTuple(Record record)
		{
			string tuple = "(";

			int size = record.Keys.Length - 1;
			
			for (int x = 0; x < size; x++)
				tuple += $"{record.Keys[x]}={QueryBuilder.GetValueString(record.Values[x])}, ";
			
			
			tuple += $"{record.Keys[size]}={QueryBuilder.GetValueString(record.Values[size])})";

			return tuple;
		}

		public static string GetValueFunctionString(Hashtable data)
		{
			int size = data.Keys.Count;
	
			string valueFunctionString = "VALUES(";
		
			string[] keys = new string[size];
			object[] values = new object[size]; 
			
			data.Keys.CopyTo(keys, 0);
			data.Values.CopyTo(values, 0);
			
			for (int x = 0; x < size - 1; x++)
				valueFunctionString += $"{QueryBuilder.GetValueString(values[x])}, "; 
			
			valueFunctionString += $"{QueryBuilder.GetValueString(values[size])})"; 

			return valueFunctionString;
		}

		public static string GetValueFunctionString(Record data)
		{
			int size = data.Values.Length;
	
			string valueFunctionString = "VALUES(";

			for (int x = 0; x < size - 1; x++)
				valueFunctionString += $"{QueryBuilder.GetValueString(data.Values[x])}, ";
			valueFunctionString += $"{QueryBuilder.GetValueString(data.Values[size - 1])})";

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
	
			for (int x = 0; x < size - 1; x++)
				setString += $"{record.Keys[x]}={QueryBuilder.GetValueString(record.Values[x])}, ";

			setString += $"{record.Keys[size - 1]}={QueryBuilder.GetValueString(record.Values[size - 1])}";
			
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

}