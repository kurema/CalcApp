using System;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public class OpAddExpression:IExpression
    {
        public OpAddExpression(IExpression left, IExpression right)
        {
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
        }

        public IExpression Right { get; }

        public IExpression Left { get; }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression, (a, b) => new FormulaExpression(a.Left, a.Right, b));
        }

        public IExpression Expand(int PowerLevel = int.MaxValue) => new FormulaExpression(Left.Expand(), Right.Expand());

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            return Left.Format(environment).Add(Right.Format(environment));
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new OpAddExpression(func(this.Left), func(this.Right));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression, () => this.MemberSelect(a => a.Multiply(expression)));
        }

        public IExpression Power(IExpression exponent)
        {
            return Helper.ExpressionPower(this, exponent);
        }

        public override string ToString()
        {
            return Left.ToString() + "+" + Right.ToString();
        }
    }
}
