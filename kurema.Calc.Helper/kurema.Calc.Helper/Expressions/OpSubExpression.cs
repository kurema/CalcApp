using System;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public class OpSubExpression : IExpression
    {
        public override string ToString()
        {
            return Left.ToString() + "-" + Right.ToString();
        }

        public OpSubExpression(IExpression left, IExpression right)
        {
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
        }

        public IExpression Right { get; }

        public IExpression Left { get; }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            return Left.Format(environment).Add(Right.Format(environment).Multiply(NumberExpression.MinusOne));
        }

        public IExpression GetRightAsMinus()
        {
            return Right.Multiply(NumberExpression.MinusOne);
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression, () => new FormulaExpression(Left, GetRightAsMinus(), expression));
        }

        public IExpression Multiply(IExpression expression)
        {
            return MemberSelect((a) => a.Multiply(expression));
        }

        public IExpression Power(IExpression exponent)
        {
            return Helper.ExpressionPower(this, exponent);
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new OpSubExpression(func(Left), func(Right));
        }

        public IExpression Expand(int PowerLevel = int.MaxValue)
        {
            return MemberSelect(a => a.Expand());
        }
    }
}
