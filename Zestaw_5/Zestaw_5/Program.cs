using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
namespace Zestaw_5
{
    public class ComparerAdapter : IComparer
    {
        Func<int, int, int> _comparer;
        public ComparerAdapter(Func<int, int, int> comparer)
        {
            _comparer = comparer;
        }
        public int Compare(object x, object y)
        {
            return _comparer((int)x, (int)y);
        }
    }

    class Program
    {
        /* this is the Comparison<int> to be converted */
        static int IntComparer(int x, int y)
        {
            return x.CompareTo(y);
        }

        static void Main(string[] args)
        {
            IComparer cmp = new ComparerAdapter(IntComparer);
            ArrayList a = new ArrayList() { 1, 5, 3, 3, 2, 4, 3 };
            /* the ArrayList's Sort method accepts ONLY an IComparer */
            a.Sort(cmp);
            foreach (var item in a)
                Console.WriteLine(item);
        }
        
    }
}
