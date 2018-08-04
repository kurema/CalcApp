using System;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public class OpFactorialExpression : IExpression
    {
        public OpFactorialExpression(IExpression n)
        {
            N = n ?? throw new ArgumentNullException(nameof(n));
        }

        public IExpression N { get; }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression, () => GetExpression().Add(expression));
        }


        public IExpression GetExpression()
        {
            if (N is NumberExpression n)
            {
                var num = n.Content.GetInt();
                if (num.WithinRange && num.Precise)
                {
                    var f = MathEx.Factorial(num.Value);
                    if (f >= 0) return this;
                }
                return this;
            }
            return this;
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            var n = N.Format(environment);
            if (n is NumberExpression number)
            {
                var a = number.Content.GetInt();
                if (a.WithinRange)
                {
                    return new NumberExpression(new NumberDecimal(MathEx.Factorial(a.Value), 0));
                }
            }
            return new OpFactorialExpression(n);
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new OpFactorialExpression(func(N));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression, () => GetExpression().Multiply(expression));
        }

        public IExpression Power(IExpression exponent)
        {
            return Helper.ExpressionPower(this.GetExpression(), exponent);
        }

        public override string ToString()
        {
            return N.ToString() + "!";
        }
    }
}
