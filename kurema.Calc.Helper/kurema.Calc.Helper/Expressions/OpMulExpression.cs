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

        public IValue Evaluate(Environment.Environment environment)
        {
            return Left.Evaluate(environment).Multiply(Right.Evaluate(environment));
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            var RightF = Right.Format(environment);
            var LeftF = Left.Format(environment);

            var rn = RightF as NumberExpression;
            var ln = LeftF as NumberExpression;

            {
                if (rn != null && ln != null)
                {
                    return new NumberExpression(rn.Content.Multiply(ln.Content));
                }
                if ((rn != null && rn.Content.Equals(NumberDecimal.Zero)) || (ln != null && ln.Content.Equals(NumberDecimal.Zero)))
                {
                    return new NumberExpression(NumberDecimal.Zero);
                }
            }

            TermExpression lt = (LeftF as TermExpression) ?? (LeftF as VariableExpression);
            TermExpression rt = (RightF as TermExpression) ?? (RightF as VariableExpression);

            if (rn != null && lt != null)
            {
                return lt.Multiply(rn.Content);
            }
            if (ln != null && rt != null)
            {
                return rt.Multiply(ln.Content);
            }
            if (rt != null && lt != null)
            {
                return rt.Multiply(lt);
            }
            return new OpMulExpression(LeftF, RightF);
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this,expression,()=> Left.Multiply(Right).Add(expression));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression, () => Left.Multiply(Right).Multiply(expression));
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
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
    }
}
