using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Zestaw_6
{
    public class Context
    {
        Dictionary<string, bool> variables;
        public Context()
        {
            variables = new Dictionary<string, bool>();
        }
        public bool GetValue(string VariableName)
        {
            return variables[VariableName];
        }
        public bool SetValue(string VariableName, bool Value)
        {
            variables[VariableName] = Value;
            return Value;
        }
    }

    public abstract class AbstractExpression
    {
        public abstract bool Interpret(Context context);
    }

    public abstract class BinaryExpression : AbstractExpression
    {
        protected AbstractExpression Arg1, Arg2;
        public BinaryExpression(AbstractExpression arg1, AbstractExpression arg2)
        {
            Arg1 = arg1; Arg2 = arg2;
        }
    }

    public class Constant : AbstractExpression
    {
        bool Value;
        public Constant(bool val)
        {
            Value = val;
        }
        public override bool Interpret(Context context)
        {
            return Value;
        }
    }

    public class Variable : AbstractExpression
    {
        string Name;
        public Variable(string name)
        {
            Name = name;
        }
        public override bool Interpret(Context context)
        {
            return context.GetValue(Name);
        }
    }

    public class Negation : AbstractExpression
    {
        AbstractExpression Arg;
        public Negation(AbstractExpression arg)
        {
            Arg = arg;
        }
        public override bool Interpret(Context context)
        {
            return !Arg.Interpret(context);
        }
    }

    public class Conjunction : BinaryExpression
    {
        public Conjunction(AbstractExpression arg1, AbstractExpression arg2) : base(arg1, arg2) { }
        public override bool Interpret(Context context)
        {
            return Arg1.Interpret(context) && Arg2.Interpret(context);
        }
    }

    public class Disjunction : BinaryExpression
    {
        public Disjunction(AbstractExpression arg1, AbstractExpression arg2) : base(arg1, arg2) { }
        public override bool Interpret(Context context)
        {
            return Arg1.Interpret(context) || Arg2.Interpret(context);
        }
    }


    [TestFixture]
    public class ContextTest
    {
        [Test]
        public void SettingVariable()
        {
            Context ctx = new Context();
            ctx.SetValue("x", false);
            ctx.SetValue("y", true);
            Assert.IsFalse(ctx.GetValue("x"));
            Assert.IsTrue(ctx.GetValue("y"));
        }
        [Test]
        [ExpectedException]
        public void NotAssignedVariable()
        {
            Context ctx = new Context();
            ctx.GetValue("z");
        }
    }

    [TestFixture]
    public class InterpreterTest
    {
        Context ctx;
        AbstractExpression v1, v2;
        [SetUp]
        public void CreateContext()
        {
            ctx = new Context();
            ctx.SetValue("x", false);
            ctx.SetValue("y", true);
            v1 = new Variable("x");
            v2 = new Variable("y");
        }
        [Test]
        public void Constant()
        {
            var trueConst = new Constant(true);
            var falseConst = new Constant(false);
            Assert.IsTrue(trueConst.Interpret(ctx));
            Assert.IsFalse(falseConst.Interpret(ctx));
        }
        [Test]
        public void BoundVariable()
        {
            Assert.IsTrue(v2.Interpret(ctx));
            Assert.IsFalse(v1.Interpret(ctx));
        }
        [Test]
        [ExpectedException]
        public void UnboundVariable()
        {
            var v = new Variable("foo");
            v.Interpret(ctx);
        }
        [Test]
        public void Negation()
        {
            var exp1 = new Negation(v1);
            var exp2 = new Negation(v2);
            Assert.IsTrue(exp1.Interpret(ctx));
            Assert.IsFalse(exp2.Interpret(ctx));
        }
        [Test]
        public void Conjunction()
        {
            var exp = new Conjunction(v1, v2);
            Assert.IsFalse(exp.Interpret(ctx));
        }
        [Test]
        public void Disjunction()
        {
            var exp = new Disjunction(v1, v2);
            Assert.IsTrue(exp.Interpret(ctx));
        }
        [Test]
        [ExpectedException]
        public void ExpressionWithUnboundVariable()
        {
            var exp = new Disjunction(v1, new Variable("z"));
            exp.Interpret(ctx);
        }
    }




}
