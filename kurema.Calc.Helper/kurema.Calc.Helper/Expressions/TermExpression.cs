using System;
using System.Collections.Generic;
using kurema.Calc.Helper.Values;

using System.Linq;

namespace kurema.Calc.Helper.Expressions
{
    public class TermExpression : IExpression
    {
        public override string ToString()
        {
            var result = new List<string>();
            if (!Coefficient.Equals(NumberDecimal.One)) result.Add(Coefficient.ToString());
            result.AddRange(Variables.Select(a => a.ToString()));
            return string.Join("", result);
        }

        public TermExpression(IValue coefficient, params VariablePowExpression[] variables)
        {
            Coefficient = coefficient ?? new NumberDecimal(1,0);
            Variables = variables.OrderBy(a=>a.Variable.Variable).ToArray() ?? new VariablePowExpression[0];
        }

        public TermExpression(IValue coefficient, params VariableExpression[] variables)
        {
            Coefficient = coefficient ?? new NumberDecimal(1, 0);
            Variables = variables.OrderBy(a => a.Variable).Select(a => new VariablePowExpression(a)).ToArray() ?? new VariablePowExpression[0];
        }

        public IValue Coefficient { get; }
        public VariablePowExpression[] Variables { get; } = new VariablePowExpression[0];

        public IValue Evaluate(Environment.Environment environment)
        {
            IValue result = Coefficient;
            foreach(var item in Variables)
            {
                result.Multiply(item.Evaluate(environment));
            }
            return result;
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment) => this;

        public static TermExpression AddIfCombinedable(TermExpression a, TermExpression b)
        {
            if (a.Variables.Count() != b.Variables.Count()) return null;
            for(int i = 0; i < a.Variables.Count(); i++)
            {
                if (a.Variables[i].Variable.Variable != b.Variables[i].Variable.Variable) return null;
                if (!a.Variables[i].Exponent.Content.Equals( b.Variables[i].Exponent.Content)) return null;
            }
            return new TermExpression(a.Coefficient.Add(b.Coefficient), a.Variables);
        }


        public TermExpression Multiply(TermExpression value)
        {
            var result = this.Multiply(value.Coefficient);
            foreach (var item in value.Variables)
            {
                result = result.Multiply(item);
            }
            return result;
        }

        public TermExpression Multiply(IValue value)
        {
            return new TermExpression(this.Coefficient.Multiply(value), this.Variables);
        }

        public TermExpression Multiply(params VariablePowExpression[] values)
        {
            var result = this.Variables.ToList();
            foreach (var value in values)
            {
                if (result.Count(a => a.Variable.Variable == value.Variable.Variable) == 0)
                {
                    result.Add(value);
                }
                else
                {
                    result = result
                        .Select(a => a.Variable.Variable != value.Variable.Variable ? a :
                            new VariablePowExpression(a.Variable, new NumberExpression(a.Exponent.Content.Add(value.Exponent.Content)))
                    ).ToList();
                }
            }
            return new TermExpression(this.Coefficient, result.ToArray());
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression, () => new OpAddExpression(this, expression));
        }

        public IExpression Multiply(IExpression ex)
        {
            return Helper.ExpressionMul(this, ex, ()=>{
                switch (ex)
                {
                    case NumberExpression expression: return new TermExpression(this.Coefficient.Multiply(expression.Content), this.Variables);
                    case VariablePowExpression expression: return this.Multiply(expression);
                    case VariableExpression expression: return this.Multiply(new VariablePowExpression(expression));
                    case TermExpression expression:
                        {
                            var result = this.Multiply(expression.Coefficient);
                            return result.Multiply(expression.Variables);
                        }
                    default: return new OpMulExpression(this, ex);
                }
            });
        }


        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return func(this);
        }

        public static implicit operator TermExpression(VariablePowExpression value)
        {
            if (value == null) return null;
            return new TermExpression(NumberDecimal.One, value);
        }

        public static implicit operator TermExpression(VariableExpression value)
        {
            if (value == null) return null;
            return new TermExpression(NumberDecimal.One, value);
        }
    }
}
