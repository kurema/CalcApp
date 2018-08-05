using System;
using kurema.Calc.Helper.Environment;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public class NumberExpression : IExpression
    {
        public NumberExpression(IValue content)
        {
            this.Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public IValue Content { get; }

        public NumberExpression Add(IValue value)
        {
            return new NumberExpression(this.Content.Add(value));
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression, () =>
            {
                switch (expression)
                {
                    case NumberExpression number: return new NumberExpression(this.Content.Add(number.Content));
                    case FormulaExpression formula: return formula.Add(this);
                    default:
                        return new FormulaExpression(expression, this);
                }
            });
        }

        public NumberExpression Format()
        {
            return this;
        }

        public NumberExpression Format(Environment.Environment environment) => Format();

        public IValue GetNumber()
        {
            return Content;
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return func(this);
        }

        public IExpression Multiply(IExpression expression)
        {
            if (this.Content.Equals( NumberDecimal.One)) return expression;
            if (this.Content.Equals(NumberDecimal.Zero)) return NumberExpression.Zero;
            return Helper.ExpressionMul(this, expression, () =>
            {
                switch (expression)
                {
                    case NumberExpression number: return new NumberExpression(this.Content.Multiply(number.Content));
                    case ArgumentExpression argument: return argument.MemberSelect(a => a.Multiply(this));
                    case FormulaExpression formula: return formula.Multiply(this);
                    default:
                        return new TermExpression(expression, this);
                }
            });
        }

        public override string ToString()
        {
            return Content.ToString();
        }

        public IExpression Power(IExpression exponent)
        {
            return Helper.ExpressionPower(this, exponent, (i) => new NumberExpression(this.Content.Power(i)), () => new OpPowExpression(this, exponent));
        }

        IExpression IExpression.Format()
        {
            return Format();
        }

        IExpression IExpression.Format(Environment.Environment environment)
        {
            return Format(environment);
        }

        public IExpression Expand(int PowerLevel = int.MaxValue) => this;

        public static NumberExpression Zero => new NumberExpression(NumberDecimal.Zero);
        public static NumberExpression One => new NumberExpression(NumberDecimal.One);
        public static NumberExpression MinusOne => new NumberExpression(NumberDecimal.MinusOne);
    }
}
