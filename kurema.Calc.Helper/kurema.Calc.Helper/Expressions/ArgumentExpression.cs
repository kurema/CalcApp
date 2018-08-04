using System;
using System.Collections.Generic;
using kurema.Calc.Helper.Values;

using System.Linq;

namespace kurema.Calc.Helper.Expressions
{
    public class ArgumentExpression : IExpression
    {
        public override string ToString()
        {
            return String.Join(",", Arguments.Select(a => a.ToString()));
        }

        public IExpression[] Arguments { get; }

        public ArgumentExpression(IExpression left, IExpression right)
        {
            left = left ?? throw new ArgumentNullException(nameof(left));
            right = right ?? throw new ArgumentNullException(nameof(right));
            List<IExpression> result;
            if(left is ArgumentExpression expressionL)
            {
                result = expressionL.Arguments.ToList();
            }
            else
            {
                result = new List<IExpression>() { left };
            }
            if (right is ArgumentExpression expressionR)
            {
                result.AddRange(expressionR.Arguments);
            }
            else
            {
                result.Add(right);
            }
            this.Arguments = result.ToArray();
        }

        public ArgumentExpression(IExpression[] arguments)
        {
            Arguments = arguments ?? new IExpression[0];
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            return new ArgumentExpression(this.Arguments.Select(a => a.Format(environment)).ToArray());
        }

        public ArgumentExpression MemberSelect(Func<IExpression,IExpression> converter)
        {
            return new ArgumentExpression(this.Arguments.Select(a => converter(a)).ToArray());
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression);
        }

        public IExpression Multiply(IExpression expression)
        {
            return MemberSelect((a) => a.Multiply(expression));
        }

        IExpression IExpression.MemberSelect(Func<IExpression, IExpression> func)
        {
            return MemberSelect(func);
        }

        public IExpression Power(IExpression expression)
        {
            return new OpPowExpression(this, expression);
        }
    }
}
