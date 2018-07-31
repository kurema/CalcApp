using System;
using System.Collections.Generic;
using System.Text;

using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public interface IExpression
    {
        IValue Evaluate();
    }

    public class NumberExpression : IExpression
    {
        private IValue content;

        public NumberExpression(IValue content)
        {
            this.content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public IValue Evaluate()
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

        public IValue Evaluate()
        {
            return left.Evaluate().Add(right.Evaluate());
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

        public IValue Evaluate()
        {
            return left.Evaluate().Substract(right.Evaluate());
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

        public IValue Evaluate()
        {
            return left.Evaluate().Multiply(right.Evaluate());
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

        public IValue Evaluate()
        {
            return left.Evaluate().Divide(right.Evaluate());
        }
    }

    public class FuncExpression : IExpression
    {
        private string name;
        private IExpression[] arg;

        public FuncExpression(string name, IExpression[] arg)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.arg = arg ?? throw new ArgumentNullException(nameof(arg));
        }

        public IValue Evaluate()
        {
            throw new NotImplementedException();
        }
    }
}
