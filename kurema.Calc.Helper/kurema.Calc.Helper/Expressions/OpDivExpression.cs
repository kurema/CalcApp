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

        public IExpression Format(Environment.Environment environment)
        {
            var leftF = Left.Format();
            var rightF = Right.Format();

            {
                if (leftF is NumberExpression leftN && rightF is NumberExpression rightN)
                {
                    return new NumberExpression(leftN.Content.Divide(rightN.Content));
                }
            }

            return new OpDivExpression(Left.Format(), Right.Format());
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
    }
}
