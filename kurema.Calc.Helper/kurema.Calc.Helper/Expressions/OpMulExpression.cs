using System;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public class OpMulExpression : IExpression
    {
        public override string ToString()
        {
            return Left.ToString() + "*" + Right.ToString();
        }

        public OpMulExpression(IExpression left, IExpression right)
        {
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
        }

        public IExpression Right { get; }

        public IExpression Left { get; }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            return Left.Format().Multiply(Right.Format());
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this,expression,()=> Left.Multiply(Right).Add(expression));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression, () => Left.Multiply(Right).Multiply(expression));
        }

        public OpMulExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new OpMulExpression(func(Left), func(Right));
        }

        public static IExpression GetNegate(IExpression expression)
        {
            return NumberExpression.MinusOne.Multiply(expression);
        }

        public IExpression Power(IExpression exponent)
        {
            return Helper.ExpressionPower(this, exponent, () => Left.Power(exponent).Multiply(Right.Power(exponent)));
        }

        IExpression IExpression.MemberSelect(Func<IExpression, IExpression> func)
        {
            return MemberSelect(func);
        }
    }
}
