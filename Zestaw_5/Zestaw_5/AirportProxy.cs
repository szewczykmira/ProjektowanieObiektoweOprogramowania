using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Zestaw_5
{
    public class Plane { }

    public class AirportPool
    {
        private static List<Plane> Planes = new List<Plane>();
        static AirportPool()
        {
            Planes.Add(new Plane());
            Planes.Add(new Plane());
            Planes.Add(new Plane());
        }
        public int NumberOfPlanesLeft
        {
            get { return Planes.Count; }
        }
        public Plane AcquirePlane()
        {
            if (NumberOfPlanesLeft == 0)
            {
                throw new System.ApplicationException("No more planes");
            }
            Plane p = Planes[0];
            Planes.RemoveAt(0);
            return p;
        }
        public void ReleasePlane(Plane p)
        {
            Planes.Add(p);
        }
    }

    public class AirportProxy
    {
        private AirportPool airport;
        private readonly Func<DateTime> _nowProvider;
        public AirportProxy(Func<DateTime> nowProvider)
        {
            _nowProvider = nowProvider;
            airport = new AirportPool();
        }
        private void CheckAccess()
        {
            DateTime time = _nowProvider();
            if (time.Hour < 8 || time.Hour > 22)
                throw new System.ApplicationException("Method accessible only at 8-22");
        }
        public int NumberOfPlanesLeft
        {
            get { return airport.NumberOfPlanesLeft; }
        }
        public Plane AcquirePlane()
        {
            CheckAccess();
            return airport.AcquirePlane();
        }
        public void ReleasePlane(Plane p)
        {
            CheckAccess();
            airport.ReleasePlane(p);
        }
    }



    [TestFixture]
    public class AirportTest
    {
        AirportProxy airport;
        private DateTime ValidTime()
        {
            return new DateTime(2013, 04, 22, 9, 00, 00);
        }
        private DateTime InvalidTime()
        {
            return new DateTime(2013, 04, 22, 7, 00, 00);
        }
        [SetUp]
        public void CreateAirport()
        {
            airport = new AirportProxy(ValidTime);
        }
        [Test]
        public void AcquireAndReleasePlane()
        {
            Assert.AreEqual(airport.NumberOfPlanesLeft, 3);
            Plane p = airport.AcquirePlane();
            Assert.AreEqual(airport.NumberOfPlanesLeft, 2);
            airport.ReleasePlane(p);
            Assert.AreEqual(airport.NumberOfPlanesLeft, 3);
        }
        [Test]
        [ExpectedException]
        public void EmptyAirport()
        {
            airport.AcquirePlane();
            airport.AcquirePlane();
            airport.AcquirePlane();
            airport.AcquirePlane();
        }
        [Test]
        [ExpectedException]
        public void AccessAtInvalidTime()
        {
            airport = new AirportProxy(InvalidTime);
            airport.AcquirePlane();
        }
    }



}
