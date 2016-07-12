using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Xml;

namespace Strategy
{
    public class DataAccessHandler
    {
        public object Result;
        Strategy _strategy;

        public DataAccessHandler(Strategy s)
        {
            _strategy = s;
        }
        public void Execute()
        {
            _strategy.Connect();
            _strategy.GetData();
            _strategy.ProcessData();
            _strategy.Disconnect();
            Result = _strategy.GetResult();
        }
    }

    public interface Strategy
    {
        object GetResult();
        void Connect();
        void GetData();
        void ProcessData();
        void Disconnect();
    }

    public class XmlStrategy : Strategy
    {
        string Result = "";
        FileStream file;
        XmlReader reader;
        public void Connect()
        {
            file = File.Open("test.xml", FileMode.Open);
        }
        public void GetData()
        {
            reader = XmlReader.Create(file);
        }
        public void ProcessData()
        {
            while (reader.Read())
            {
                string name = reader.Name;
                if (name.Length > Result.Length)
                    Result = name;
            }
        }
        public void Disconnect()
        {
            file.Close();
        }
        public object GetResult()
        {
            return Result;
        }
    }

    public class SqlStrategy : Strategy
    {
        int Result = 0;
        SqlDataReader reader;
        SqlConnection dbcon;
        SqlCommand dbcmd;
        public void Connect()
        {
            dbcon = new SqlConnection("file:test.db");
            dbcon.Open();
        }
        public void GetData()
        {
            dbcmd = dbcon.CreateCommand();
            dbcmd.CommandText = "SELECT area FROM countries";
            reader = dbcmd.ExecuteReader();
        }
        public void ProcessData()
        {
            while (reader.Read())
            {
                var data = reader.GetInt32(0);
                Result += data;
            }
        }
        public void Disconnect()
        {
            reader.Close();
            dbcmd.Dispose();
            dbcon.Close();
        }
        public object GetResult()
        {
            return Result;
        }
    }

    public static class Client
    {
        public static void Main()
        {
            var xml = new DataAccessHandler(new XmlStrategy());
            xml.Execute();
            Console.WriteLine("Longest name in XML file: {0}", xml.Result);
            Console.ReadLine();
            var db = new DataAccessHandler(new SqlStrategy());
            //db.Execute();
            Console.WriteLine("Sum of values in area column: {0}", db.Result);
        }
    }
}
