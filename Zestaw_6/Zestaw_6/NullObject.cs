using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zestaw_6
{

    public interface ILogger
    {
        void Log(string Message);
    }

    public enum LogType { None, Console, File }

    public class NullLogger : ILogger
    {
        public void Log(string Message) { }
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string Message)
        {
            Console.WriteLine(Message);
        }
    }

    public class FileLogger : ILogger
    {
        string fileName;
        public FileLogger(string FileName)
        {
            fileName = FileName;
        }
        public void Log(string Message)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(fileName);
            file.WriteLine(Message);
            file.Close();
        }
    }

    public class LoggerFactory
    {
        public static ILogger GetLogger(LogType logType, string Parameters = null)
        {
            switch (logType)
            {
                case LogType.None:
                    return new NullLogger();
                case LogType.Console:
                    return new ConsoleLogger();
                case LogType.File:
                    return new FileLogger(Parameters);
            }
            throw new ArgumentException("Unknown logger type", "logType");
        }
    }
}
