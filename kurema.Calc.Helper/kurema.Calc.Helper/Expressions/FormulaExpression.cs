using System;
using System.Collections.Generic;
using kurema.Calc.Helper.Values;

using System.Linq;
using kurema.Calc.Helper.Environment;

namespace kurema.Calc.Helper.Expressions
{
    public class FormulaExpression : IExpression
    {
        public IValue Value { get; }
        public TermExpression[] Terms { get; }
        public IExpression[] Other { get; }

        public override string ToString()
        {
            var result = new List<string>();
            if (!Value.Equals(NumberDecimal.Zero)) result.Add(Value.ToString());
            result.AddRange(Terms.Select(a => a.ToString()));
            result.AddRange(Other.Select(a => a.ToString()));
            return string.Join("+", result);
        }

        public FormulaExpression(IValue value, TermExpression[] terms, IExpression[] other)
        {
            this.Value = value ?? NumberDecimal.Zero;
            this.Terms = terms ?? new TermExpression[0];
            this.Other = other ?? new IExpression[0];
        }

        public FormulaExpression()
        {
            this.Value = NumberDecimal.Zero;
            this.Terms = new TermExpression[0];
            this.Other = new IExpression[0];
        }

        public FormulaExpression(params IExpression[] expressions)
        {
            var result = new FormulaExpression();
            foreach(var item in expressions)
            {
                result = result.Add(item);
            }
            this.Value = result.Value;
            this.Terms = result.Terms;
            this.Other = result.Other;
        }

        public FormulaExpression Format(Environment.Environment environment) => Format();

        public FormulaExpression Format() => MemberSelect(a => a.Format());

        public FormulaExpression Add(IValue value)
        {
            return new FormulaExpression(this.Value.Add(value), Terms, Other);
        }

        public FormulaExpression Add(NumberExpression value)
        {
            return this.Add(value.Content);
        }

        public FormulaExpression Add(params TermExpression[] terms)
        {
            var termList = this.Terms.ToList();
            foreach(var term in terms){
                for (int i = 0; i < termList.Count(); i++)
                {
                    var temp = TermExpression.AddIfCombinedable(termList[i], term);
                    if (temp != null)
                    {
                        termList[i] = temp;
                        goto BREAKER;
                    }
                }
                termList.Add(term);
                BREAKER:;
            }
            return new FormulaExpression(Value, termList.ToArray(), Other);
        }

        public FormulaExpression Add(FormulaExpression value)
        {
            var result = this;
            result = result.Add(value.Value);
            result = result.Add(value.Terms);
            var other = result.Other.ToList();
            other.AddRange(value.Other);
            result = new FormulaExpression(result.Value, result.Terms, other.ToArray());
            return result;
        }

        public FormulaExpression Add(IExpression expression)
        {
            expression = expression.Format();
            switch (expression)
            {
                case NumberExpression n:return this.Add(n);
                case TermExpression n:return this.Add(n);
                case VariableExpression n:return this.Add(n);
                case VariablePowExpression n:return this.Add(n);
                case FormulaExpression n:return this.Add(n);
                default:
                    var other = this.Other.ToList();
                    other.Add(expression);
                    return new FormulaExpression(this.Value, this.Terms, other.ToArray());
            }
        }

        public FormulaExpression Multiply(IValue value)
        {
            return new FormulaExpression(this.Value.Multiply(value),
                Terms.Select(a=>a.Multiply(value)).ToArray(),
                Other.Select(a=>a.Multiply(new NumberExpression(value))).ToArray());
        }

        public FormulaExpression Multiply(NumberExpression value)
        {
            return this.Multiply(value.Content);
        }

        IExpression IExpression.Add(IExpression expression)
        {
            return Add(expression);
        }

        public IExpression Multiply(IExpression expression)
        {
            { if (expression is NumberExpression number && number.Content.Equals(NumberDecimal.Zero)) return NumberExpression.Zero; }
            { if (expression is NumberExpression number && number.Content.Equals(NumberDecimal.One)) return this; }
            if (expression is NumberExpression n) return Multiply(n.Content);
            return MemberSelect(a => a.Multiply(expression));
        }

        public FormulaExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            var c = GetMembers().Select(a => func(a)).ToArray();
            return new FormulaExpression(c);
        }

        public IExpression[] GetMembers()
        {
            List<IExpression> expressions = new List<IExpression>();
            if (!this.Value.Equals(NumberDecimal.Zero)) expressions.Add(new NumberExpression(this.Value));
            expressions.AddRange(this.Terms);
            expressions.AddRange(this.Other);
            return expressions.ToArray();
        }

        IExpression IExpression.MemberSelect(Func<IExpression, IExpression> func)
        {
            return MemberSelect(func);
        }

        public IExpression Power(IExpression exponent)
        {
            return Helper.ExpressionPower(this, exponent);
        }

        IExpression IExpression.Format()
        {
            return Format();
        }

        IExpression IExpression.Format(Environment.Environment environment)
        {
            return Format(environment);
        }

        public IExpression Expand(int PowerLevel = 3)
        {
            return MemberSelect(a => a.Expand());
        }
    }
}
