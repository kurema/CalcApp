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
        public IExpression[] Others { get; }

        public bool IsZero => Terms.All(a => a.IsZero) && Others.All(a => a.IsZero) && Value.Equals(NumberDecimal.Zero);

        public override string ToString()
        {
            var result = new List<string>();
            if (!Value.Equals(NumberDecimal.Zero)) result.Add(Value.ToString());
            result.AddRange(Terms.Select(a => a.ToString()));
            result.AddRange(Others.Select(a => a.ToString()));
            return string.Join("+", result);
        }

        public FormulaExpression(IValue value, TermExpression[] terms, IExpression[] other)
        {
            this.Value = value ?? NumberDecimal.Zero;
            this.Terms = terms.Where(a=>!a.IsZero).ToArray() ?? new TermExpression[0];
            this.Others = other.Where(a => !a.IsZero).ToArray() ?? new IExpression[0];
        }

        public FormulaExpression()
        {
            this.Value = NumberDecimal.Zero;
            this.Terms = new TermExpression[0];
            this.Others = new IExpression[0];
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
            this.Others = result.Others;
        }

        public IExpression Format(Environment.Environment environment) => Format();

        public IExpression Format() => MemberSelect(a => a.Format());

        public FormulaExpression Add(IValue value)
        {
            return new FormulaExpression(this.Value.Add(value), Terms, Others);
        }

        public FormulaExpression Add(NumberExpression value)
        {
            if (value.Content.Equals(NumberDecimal.Zero)) return this;
            else return this.Add(value.Content);
        }

        public FormulaExpression Add(params TermExpression[] terms)
        {
            var termList = this.Terms.ToList();
            foreach(var term in terms){
                if (term.Coefficient.Equals(NumberDecimal.Zero))
                    continue;
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
            return new FormulaExpression(Value, termList.ToArray(), Others);
        }

        public FormulaExpression Add(FormulaExpression value)
        {
            var result = this;
            result = result.Add(value.Value);
            result = result.Add(value.Terms);
            var other = result.Others.ToList();
            other.AddRange(value.Others);
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
                    var other = this.Others.ToList();
                    other.Add(expression);
                    return new FormulaExpression(this.Value, this.Terms, other.ToArray());
            }
        }

        public IExpression GetSingleOrDefault()
        {
            if (this.Others.Count() > 0) return null;
            if (this.Terms.Count() == 0)
            {
                return new NumberExpression(this.Value);
            }
            if (this.Value.Equals(NumberDecimal.Zero) && this.Terms.Count()==1)
            {
                return this.Terms[0];
            }
            return null;
        }

        public FormulaExpression Multiply(IValue value)
        {
            return new FormulaExpression(this.Value.Multiply(value),
                Terms.Select(a=>a.Multiply(value)).ToArray(),
                Others.Select(a=>a.Multiply(new NumberExpression(value))).ToArray());
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
            //return new FormulaExpression(GetMembers().Select(a => a.Multiply(expression)).ToArray());
            return new TermExpression(this, expression);
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            var c = GetMembers().Select(a => func(a)).ToArray();
            var f = new FormulaExpression(c);
            var single = f.GetSingleOrDefault();
            if (single == null) return f; else return single;
        }

        public IExpression[] GetMembers()
        {
            List<IExpression> expressions = new List<IExpression>();
            if (!this.Value.Equals(NumberDecimal.Zero)) expressions.Add(new NumberExpression(this.Value));
            expressions.AddRange(this.Terms);
            expressions.AddRange(this.Others);
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

        //IExpression IExpression.Format()
        //{
        //    return Format();
        //}

        //IExpression IExpression.Format(Environment.Environment environment)
        //{
        //    return Format(environment);
        //}

        public IExpression Expand(int PowerLevel = 3)
        {
            return MemberSelect(a => a.Expand());
        }
    }
}
