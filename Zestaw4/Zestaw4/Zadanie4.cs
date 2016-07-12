using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Zestaw4
{
    public class NoPlanesException : System.Exception
    {
        public NoPlanesException() : base() { }
    }
    public class Plane
    { }
    public class Airport
    {
        private static List<Plane> UsedPlanes;
        private static Stack<Plane> FreePlanes;

        public Airport(int value)
        {
            UsedPlanes = new List<Plane>(value);
            FreePlanes = new Stack<Plane>(value);
            for (int i = 0; i < value; i++)
            {
                FreePlanes.Push(new Plane());
            }

        }

        public Plane AcquirePlane()
        {
            if(FreePlanes.Count() <= 0)
            {
                throw new NoPlanesException();
            }
            Plane plane = FreePlanes.Pop();
            UsedPlanes.Add(plane);
            return plane;
        }

        public void ReleasePlane(Plane plane)
        {
            if(FreePlanes.Contains(plane))
            {
                UsedPlanes.Remove(plane);
                FreePlanes.Push(new Plane());
            }
        }



    }

    [TestFixture]
    public class AirportTest
    {
        [Test]
        public void Returns_instance_of_correct_type()
        {
            Airport airport = new Airport(10);
            object plane = airport.AcquirePlane();

            Assert.IsInstanceOf<Plane>(plane);
        }

        [Test]
        [ExpectedException(typeof(NoPlanesException))]
        public void Raises_exception_when_no_more_planes_available()
        {
            Airport airport = new Airport(1);
            Plane plane1 = airport.AcquirePlane();
            Plane plane2 = airport.AcquirePlane();
        }

        [Test]
        public void Properly_releases_planes()
        {
            Airport airport = new Airport(1);
            Plane plane1 = airport.AcquirePlane();
            airport.ReleasePlane(plane1);

            Assert.DoesNotThrow(delegate { Plane plane1again = airport.AcquirePlane(); });
        }

        
    }




    class Zadanie4
    {
    }
}
