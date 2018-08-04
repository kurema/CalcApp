using System;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public interface IExpression
    {
        IExpression Format();
        IExpression Format(Environment.Environment environment);

        IExpression Add(IExpression expression);
        IExpression Multiply(IExpression expression);
        IExpression Power(IExpression exponent);

        IExpression MemberSelect(Func<IExpression, IExpression> func);
    }
}
