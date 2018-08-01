using System;
using System.Collections.Generic;
using System.Text;
using kurema.Calc.Helper.Environment;
using kurema.Calc.Helper.Values;

using System.Linq;

namespace kurema.Calc.Helper.Expressions
{
    public interface IExpression
    {
        IValue Evaluate(Environment.Environment environment);
    }

    public class NumberExpression : IExpression
    {
        private IValue content;

        public NumberExpression(IValue content)
        {
            this.content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            return content;
        }
    }

    public class OpAddExpression:IExpression
    {
        private IExpression right;
        private IExpression left;

        public OpAddExpression(IExpression left, IExpression right)
        {
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            this.left = left ?? throw new ArgumentNullException(nameof(left));
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            return left.Evaluate(environment).Add(right.Evaluate(environment));
        }
    }

    public class OpSubExpression : IExpression
    {
        private IExpression right;
        private IExpression left;

        public OpSubExpression(IExpression left, IExpression right)
        {
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            this.left = left ?? throw new ArgumentNullException(nameof(left));
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            return left.Evaluate(environment).Substract(right.Evaluate(environment));
        }
    }

    public class OpMulExpression : IExpression
    {
        private IExpression right;
        private IExpression left;

        public OpMulExpression(IExpression left, IExpression right)
        {
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            this.left = left ?? throw new ArgumentNullException(nameof(left));
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            return left.Evaluate(environment).Multiply(right.Evaluate(environment));
        }
    }

    public class OpDivExpression : IExpression
    {
        private IExpression right;
        private IExpression left;

        public OpDivExpression(IExpression left, IExpression right)
        {
            this.right = right ?? throw new ArgumentNullException(nameof(right));
            this.left = left ?? throw new ArgumentNullException(nameof(left));
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            return left.Evaluate(environment).Divide(right.Evaluate(environment));
        }
    }

    public class ArgumentExpression : IExpression
    {
        public readonly IExpression[] Arguments;

        public ArgumentExpression(IExpression left, IExpression right)
        {
            left = left ?? throw new ArgumentNullException(nameof(left));
            right = right ?? throw new ArgumentNullException(nameof(right));
            List<IExpression> result;
            if(left is ArgumentExpression expressionL)
            {
                result = expressionL.Arguments.ToList();
            }
            else
            {
                result = new List<IExpression>() { left };
            }
            if (right is ArgumentExpression expressionR)
            {
                result.AddRange(expressionR.Arguments);
            }
            else
            {
                result.Add(right);
            }
            this.Arguments = result.ToArray();
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            return Arguments[0].Evaluate(environment);
        }
    }

    public class FuncExpression : IExpression
    {
        private string name;
        private IExpression[] arg;

        public FuncExpression(string name,  IExpression arg)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.arg = arg == null ? new IExpression[0] :
                (arg is ArgumentExpression argument ? argument.Arguments : new[] { arg });
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            if(environment.Functions.ContainsKey(name))
            {
                return environment.Functions[name].Evaluate(environment, arg).Evaluate(environment);
            }
            else
            {
                throw new Exception("Function not exist");
            }
        }
    }
}
