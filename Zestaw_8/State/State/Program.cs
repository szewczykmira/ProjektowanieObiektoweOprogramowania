using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Text;
using System.Threading.Tasks;

namespace State
{

    public abstract class State
    {
        protected VendorMachine vendorMachine;
        protected int coffeeCost = 200;
        protected int insertedMoney = 0;
        public virtual void OrderCoffee()
        {
            throw new Exception("Operation invalid in current state");
        }
        public virtual void InsertCoin(int amount)
        {
            throw new Exception("Operation invalid in current state");
        }
        public virtual void TakeCoffee()
        {
            throw new Exception("Operation invalid in current state");
        }
    }

    public class VendorMachine
    {
        public State state;
        public VendorMachine()
        {
            state = new WaitingForOrder(this);
        }
        public void OrderCoffee()
        {
            state.OrderCoffee();
        }
        public void InsertCoin(int amount)
        {
            state.InsertCoin(amount);
        }
        public void TakeCoffee()
        {
            state.TakeCoffee();
        }
    }

    public class WaitingForOrder : State
    {
        public WaitingForOrder(VendorMachine vm)
        {
            vendorMachine = vm;
        }
        public override void OrderCoffee()
        {
            vendorMachine.state = new WaitingForMoney(vendorMachine);
        }
    }

    public class WaitingForMoney : State
    {
        public WaitingForMoney(VendorMachine vm)
        {
            vendorMachine = vm;
        }
        public override void InsertCoin(int amount)
        {
            insertedMoney += amount;
            if (insertedMoney >= coffeeCost)
                vendorMachine.state = new CoffeeReady(vendorMachine);
        }
    }


    public class CoffeeReady : State
    {
        public CoffeeReady(VendorMachine vm)
        {
            vendorMachine = vm;
        }
        public override void TakeCoffee()
        {
            vendorMachine.state = new WaitingForOrder(vendorMachine);
        }
    }




    class Program
    {
        static void Main(string[] args)
        {
        }
    }


    [TestFixture]
    public class StateTest
    {
        VendorMachine vm;
        [SetUp]
        public void setupVendorMachin()
        {
            vm = new VendorMachine();
        }
        [Test]
        public void OrderingCoffee()
        {
            vm.OrderCoffee();
            Assert.IsInstanceOfType(typeof(WaitingForMoney), vm.state);
        }
        [Test]
        public void PayForCoffee()
        {
            vm.OrderCoffee();
            vm.InsertCoin(100);
            Assert.IsInstanceOfType(typeof(WaitingForMoney), vm.state);
            vm.InsertCoin(50);
            Assert.IsInstanceOfType(typeof(WaitingForMoney), vm.state);
            vm.InsertCoin(50);
            Assert.IsInstanceOfType(typeof(CoffeeReady), vm.state);
        }
        [Test]
        public void WholeTransaction()
        {
            vm.OrderCoffee();
            vm.InsertCoin(200);
            vm.TakeCoffee();
            Assert.IsInstanceOfType(typeof(WaitingForOrder), vm.state);
        }
        [Test]
        [ExpectedException]
        public void TakeCoffeeBeforeOrdering()
        {
            vm.TakeCoffee();
        }
    }




}
