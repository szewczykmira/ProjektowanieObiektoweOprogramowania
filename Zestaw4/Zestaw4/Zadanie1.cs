using System;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace Zestaw4
{
    public class Singleton
    {
        
        private static Singleton _instance;
        private static readonly object _padlock = new Object();

        Singleton() { }

        public static Singleton Instance
        {
            get
            {
                if (_instance == null)
                {
                     _instance = new Singleton();
                }
              
                return _instance;
            }
        }
    }
    public class ThreadSingleton {
        private static ThreadSingleton _instance;
        ThreadSingleton() { }
        public static ThreadSingleton Instance
        {
            get {
                if(_instance==null)
                {
                    _instance = new ThreadSingleton();
                }
                return _instance;
            }

        }
    }

    public class FiveSecondSingleton
    {
        private static FiveSecondSingleton _instance;
        FiveSecondSingleton() {}
        private static DateTime _expiration;

        public static FiveSecondSingleton Instance
        {
            get
            {
                DateTime now = DateTime.Now;
                if (_expiration < now)
                {
                    _instance = new FiveSecondSingleton();
                    _expiration = now.AddSeconds(5);
                }
                return _instance;
            }
            
        }
    }
    class Zadanie1
    {
        public static int Main()
        {
            return 0;
        }
    }



    [TestFixture]
    public class SingleSingletonTest
    {
        [Test]
        public void Returns_same_instance()
        {
            Singleton singleton1 = Singleton.Instance;
            Singleton singleton2 = Singleton.Instance;

            Assert.AreSame(singleton1, singleton2);
        }
    }

    [TestFixture]
    public class ThreadSingletonTest
    {
        [Test]
        public void Returns_same_instance_for_same_thread()
        {
            ThreadSingleton singleton1 = ThreadSingleton.Instance;
            ThreadSingleton singleton2 = ThreadSingleton.Instance;

            Assert.AreSame(singleton1, singleton2);
        }
        [Test]
        public void Returns_different_instances_for_different_threads()
        {
            ThreadSingleton singleton1 = ThreadSingleton.Instance;
            ThreadSingleton singleton2 = null;
            var thread = new Thread(() =>
            {
                singleton2 = ThreadSingleton.Instance;
            });
            thread.Start();
            thread.Join();

            Assert.AreNotSame(singleton1, singleton2);
        }
    }

    [TestFixture]
    public class FiveSecondSingletonTest
    {
        [Test]
        public void Returns_same_instance_for_immediate_calls()
        {
            FiveSecondSingleton singleton1 = FiveSecondSingleton.Instance;
            FiveSecondSingleton singleton2 = FiveSecondSingleton.Instance;

            Assert.AreSame(singleton1, singleton2);
        }
        [Test]
        public void Returns_same_instance_for_within_five_seconds()
        {
            FiveSecondSingleton singleton1 = FiveSecondSingleton.Instance;
            Thread.Sleep(4000);
            FiveSecondSingleton singleton2 = FiveSecondSingleton.Instance;

            Assert.AreSame(singleton1, singleton2);
        }
        [Test]
        public void Returns_different_instances_for_more_than_five_seconds()
        {
            FiveSecondSingleton singleton1 = FiveSecondSingleton.Instance;
            Thread.Sleep(6000);
            FiveSecondSingleton singleton2 = FiveSecondSingleton.Instance;

            Assert.AreNotSame(singleton1, singleton2);
        }
    }









}
