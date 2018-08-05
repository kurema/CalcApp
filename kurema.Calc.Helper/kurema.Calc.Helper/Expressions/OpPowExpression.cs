using System;
using System.Linq;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public class OpPowExpression:IExpression
    {
        public OpPowExpression(IExpression @base, IExpression exponent)
        {
            Base = @base ?? throw new ArgumentNullException(nameof(@base));
            Exponent = exponent ?? throw new ArgumentNullException(nameof(exponent));
        }

        public IExpression Base { get; }
        public IExpression Exponent { get; }

        public IExpression Format()
        {
            return Format(null);
        }

        public IExpression Format(Environment.Environment environment)
        {
            return Base.Format(environment).Power(Exponent.Format(environment));
        }

        public override string ToString()
        {
            return Base.ToString() + "^" + Exponent.ToString();
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new OpPowExpression(func(Base), func(Exponent));
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression, () => new OpAddExpression(this, expression));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression, () => new OpMulExpression(this, expression));
        }

        public IExpression Power(IExpression exponent)
        {
            return Base.Power(Exponent).Power(exponent);
        }

        public IExpression Expand(int PowerLevel = int.MaxValue)
        {
            var cnt = Helper.GetExpressionValue(this.Exponent)?.GetInt();
            var splited = Helper.SplitAddition(this.Base).Select(a => a.Expand()).ToArray();
            var minusPowerLevel = PowerLevel == int.MaxValue ? int.MinValue : -PowerLevel;
            if (splited.Length == 1)
            {
                return splited[0].Power(this.Exponent);
            }
            if (cnt?.Healthy == true)
            {
                if (cnt.Value.Value == 0) { return this.Base; }
                if (cnt.Value.Value > 0 && cnt.Value.Value < PowerLevel) { return ExpandLevel(splited, cnt.Value.Value); }
                if (cnt.Value.Value < 0 && cnt.Value.Value > -PowerLevel) { return ExpandLevel(splited, -cnt.Value.Value).Power(NumberExpression.MinusOne); }
            }
            return MemberSelect(a => a.Expand());
        }

        public IExpression ExpandLevel(IExpression[] expressions,int currentLevel,IExpression current=null)
        {
            current = current ?? NumberExpression.One;
            IExpression result = NumberExpression.Zero;
            // This is not smart.
            if (currentLevel == 0) return current;
            foreach(var item in expressions)
            {
                result = result.Add(ExpandLevel(expressions, currentLevel - 1, current.Multiply(item)));
            }
            return result;
        }
    }
}
