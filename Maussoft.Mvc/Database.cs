using System;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace Maussoft.Mvc
{
	public class Database
	{
		//private string _connectionString;

		public Database (string connectionString)
		{
			//_connectionString = connectionString;
			//Trace.Listeners.Add(new ConsoleTraceListener());
			//MySqlTrace.

			//http://www.codeproject.com/Articles/19982/Port-Writer-Trace-Listener-for-NET-Applications

			//http://docs.oracle.com/cd/E17952_01/connector-net-en/connector-net-programming-tracing.html

			//TraceSource ts = new TraceSource("mysql");
			//ts.Switch = new SourceSwitch("MySwitch", "Verbose");
			//ts.Listeners.Add(new ConsoleTraceListener());
			//ts.Flush();
			//ts.Close();
		}

		public object Select(string query)
		{
			//MySqlCommand m = new MySqlCommand (query);
			//m.Parameters.AddWithValue ("0", 0);
			//m.Parameters.AddWithValue ("1", 1);
			//MySqlDataReader r = m.ExecuteReader();
			//if (r.HasRows)
			//	level = Convert.ToInt32(r.GetValue(0).ToString());
			//r.Close();
			return new object ();
		}
	}
}

