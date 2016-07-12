using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Zestaw_7
{
    public abstract class Department {
        protected Department _next;

        public Department SetNext(Department next) {
            this._next = next;
            return this._next;
        }

        public abstract void ProcessLetter(string request);
    }

    public class President : Department {
        public override void ProcessLetter(string request) {
            if(request.ToLower().Contains("praise"))
                Console.WriteLine("President received the letter: " + request);
            else if(this._next != null)
                this._next.ProcessLetter(request);
        }
    }

    public class LawSection : Department {
        public override void ProcessLetter(string request) {
            if(request.ToLower().Contains("complaint"))
                Console.WriteLine("Law section received the letter: " + request);
            else if(this._next != null)
                this._next.ProcessLetter(request);
        }
    }

    public class TradingSection : Department {
        public override void ProcessLetter(string request) {
            if(request.ToLower().Contains("order"))
                Console.WriteLine("Trading section received the letter: " + request);
            else if(this._next != null)
                this._next.ProcessLetter(request);
        }
    }

    public class MarketingSection : Department {
        public override void ProcessLetter(string request) {
            Console.WriteLine("Marketing section received the letter: " + request);
            if(this._next != null)
                this._next.ProcessLetter(request);
        }
    }

    public class Archiver : Department {
        private Stream _logger;

        public Archiver(Stream logger) {
            this._logger = logger;
        }

        public override void ProcessLetter(string request) {
            _logger.Write(Encoding.UTF8.GetBytes(request), 0, request.Length);
        }
    }

    public class LetterDispatcher : Department {
        public LetterDispatcher(Stream logger = null) {
            (logger != null ? this.SetNext(new Archiver(logger)) : this.SetNext(new President()))
                .SetNext(new LawSection())
                .SetNext(new TradingSection())
                .SetNext(new MarketingSection());
        }

        public override void ProcessLetter(string request) {
            this._next.ProcessLetter(request);
        }
    }


    [TestFixture]
    public class ChainOfResponsibilityTests
    {
        private TextWriter _backup;
        private MemoryStream _stream;
        private StreamWriter _writer;

        [TestFixtureSetUp]
        public void SetUp()
        {
            this._backup = Console.Out;
            this._stream = new MemoryStream();
            this._writer = new StreamWriter(this._stream);
            Console.SetOut(this._writer);
        }

        [SetUp]
        public void Init()
        {
            this._stream.SetLength(0);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            this._writer.Close();
            Console.SetOut(this._backup);
        }

        [Test]
        public void PresidentTest()
        {
            Department dispatcher = new LetterDispatcher();

            dispatcher.ProcessLetter("Praise the Loard!");
            this._writer.Flush();

            Assert.AreEqual("President received the letter: Praise the Loard!\r\n", Encoding.UTF8.GetString(this._stream.ToArray()));
        }

        [Test]
        public void LawSectionTest()
        {
            Department dispatcher = new LetterDispatcher();

            dispatcher.ProcessLetter("Complaint about everything");
            this._writer.Flush();

            Assert.AreEqual("Law section received the letter: Complaint about everything\r\n", Encoding.UTF8.GetString(this._stream.ToArray()));
        }

        [Test]
        public void TradingSectionTest()
        {
            Department dispatcher = new LetterDispatcher();

            dispatcher.ProcessLetter("Placing an order here.");
            this._writer.Flush();

            Assert.AreEqual("Trading section received the letter: Placing an order here.\r\n", Encoding.UTF8.GetString(this._stream.ToArray()));
        }

        [Test]
        public void MarketingSectionTest()
        {
            Department dispatcher = new LetterDispatcher();

            dispatcher.ProcessLetter("Tell them anything.");
            this._writer.Flush();

            Assert.AreEqual("Marketing section received the letter: Tell them anything.\r\n", Encoding.UTF8.GetString(this._stream.ToArray()));
        }

        [Test]
        public void ArchiverTest()
        {
            MemoryStream logger = new MemoryStream();
            Department dispatcher = new LetterDispatcher(logger);

            dispatcher.ProcessLetter("a random letter which goes nowhere");
            this._writer.Flush();

            Assert.AreEqual("a random letter which goes nowhere", Encoding.UTF8.GetString(logger.ToArray()));

            logger.SetLength(0);

            dispatcher.ProcessLetter("Praise to Pres");
            this._writer.Flush();

            Assert.AreEqual("Praise to Pres", Encoding.UTF8.GetString(logger.ToArray()));

            logger.SetLength(0);

            dispatcher.ProcessLetter("COMPLAINT");
            this._writer.Flush();

            Assert.AreEqual("COMPLAINT", Encoding.UTF8.GetString(logger.ToArray()));
        }
    }




}

