using System;
using System.Collections.Generic;
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
            var leftF = Left.Format(environment);
            var rightF = Right.Format(environment);
            return leftF.Multiply(rightF);
            //return Left.Format(environment).Multiply(Right.Format(environment));
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

        public IExpression Expand(int PowerLevel = int.MaxValue)
        {
            IExpression result = NumberExpression.Zero;
            var Lefts = Helper.SplitAddition(this.Left);
            var Rights = Helper.SplitAddition(this.Right);
            foreach (var item1 in Lefts)
            {
                foreach (var item2 in Rights)
                {
                    result = result.Add(item1.Multiply(item2));
                }
            }
            return result;
        }
    }
}
