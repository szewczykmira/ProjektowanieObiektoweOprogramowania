using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Zestaw_6
{
    class Program
    {
        static void Main(string[] args)
        {
            //Zad1

            ILogger logger = LoggerFactory.GetLogger(LogType.Console);
            logger.Log("foo");

            ILogger logger1 = LoggerFactory.GetLogger(LogType.File, "foo.txt");
            logger1.Log("bar");

            ILogger logger2 = LoggerFactory.GetLogger(LogType.None);
            logger2.Log("qux");

            //Zad4
            Expression<Func<int, int>> exprTree = num => num * 5;


            Console.ReadLine();
        }
    }
}
