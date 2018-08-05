using System;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public class OpDivExpression : IExpression
    {
        public override string ToString()
        {
            return Left.ToString() + "/" + Right.ToString();
        }

        public OpDivExpression(IExpression left, IExpression right)
        {
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
        }

        public IExpression Right { get; }

        public IExpression Left { get; }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment e)
        {
            return Left.Format(e).Multiply(Right.Format(e).Power(NumberExpression.MinusOne));
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression);
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression);
        }

        public IExpression Power(IExpression exponent)
        {
            return Helper.ExpressionPower(this, exponent, () => Left.Power(exponent).Multiply(Right.Power(exponent.Multiply(NumberExpression.MinusOne))));
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new OpDivExpression(func(Left), func(Right));
        }

        public IExpression Expand(int PowerLevel = int.MaxValue)
        {
            //ToDo: IMplement;
            return MemberSelect(a => a.Expand());
        }
    }
}
