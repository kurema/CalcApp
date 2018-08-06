using System;
using System.Collections.Generic;
using kurema.Calc.Helper.Values;

using System.Linq;
using kurema.Calc.Helper.Environment;

namespace kurema.Calc.Helper.Expressions
{
    public class ArgumentExpression : IExpression
    {
        public override string ToString()
        {
            return String.Join(",", Arguments.Select(a => a.ToString()));
        }

        public IExpression[] Arguments { get; }

        public bool IsZero => false;

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

        public ArgumentExpression Format() => Format(null);

        public ArgumentExpression Format(Environment.Environment environment)
        {
            return MemberSelect(a => a.Format(environment));
        }

        public ArgumentExpression MemberSelect(Func<IExpression,IExpression> converter)
        {
            return new ArgumentExpression(this.Arguments.Select(a => converter(a)).ToArray());
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression);
        }

        public ArgumentExpression Multiply(IExpression expression)
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

        IExpression IExpression.Format()
        {
            return Format();
        }

        IExpression IExpression.Format(Environment.Environment environment)
        {
            return Format(environment);
        }

        IExpression IExpression.Multiply(IExpression expression)
        {
            return Multiply(expression);
        }

        public IExpression Expand(int PowerLevel = int.MaxValue)
        {
            return MemberSelect(a => a.Expand());
        }
    }
}
