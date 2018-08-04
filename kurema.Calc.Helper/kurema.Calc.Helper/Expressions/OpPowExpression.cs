using System;
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

        public IValue Evaluate(Environment.Environment environment)
        {
            throw new NotImplementedException();
        }

        public IExpression Format()
        {
            //ToDo: fix.
            return this;
        }

        public IExpression Format(Environment.Environment environment)
        {
            //ToDo: fix.
            return new OpPowExpression(Base.Format(environment), Exponent.Format(environment));
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
    }
}
