using System;
using System.Collections.Generic;
using kurema.Calc.Helper.Values;

using System.Linq;
using kurema.Calc.Helper.Environment;

namespace kurema.Calc.Helper.Expressions
{
    public class TermExpression : IExpression
    {
        public override string ToString()
        {
            var result = new List<string>();
            if (!Coefficient.Equals(NumberDecimal.One)) result.Add(Coefficient.ToString());
            result.AddRange(Variables.Select(a => a.ToString()));
            result.AddRange(Others.Select(a =>"("+ a.ToString()+")"));
            return string.Join("*", result);
        }

        public TermExpression(IValue coefficient, params VariablePowExpression[] variables) : this(coefficient, variables, null)
        {
        }

        public TermExpression(IValue coefficient, params VariableExpression[] variables) : this(coefficient, null, variables)
        {
        }

        public TermExpression(IValue coefficient,  VariablePowExpression[] variablePows, VariableExpression[] variables,params IExpression[] others)
        {
            Coefficient = coefficient ?? new NumberDecimal(1, 0);
            if (Coefficient.Equals(NumberDecimal.Zero)) return;
            var variablesR = TermExpression.Multiply(variablePows, variables?.Select(a => (VariablePowExpression)a))?.ToList();
            var othersR = new List<IExpression>();

            if (others != null)
            {
                var result = new TermExpression(others);
                Coefficient = Coefficient.Multiply(result.Coefficient);
                variablesR.AddRange(result.Variables);
                othersR.AddRange(result.Others);
            }

            Variables = Unify(variablesR);
            Others = othersR.ToArray();
        }

        public TermExpression(params IExpression[] exs)
        {
            IValue coefficientR = NumberDecimal.One;
            var variablesR = new List<VariablePowExpression>();
            var othersR = new List<IExpression>();

            foreach (var ex in exs)
            {
                switch (ex)
                {
                    case NumberExpression expression: coefficientR = coefficientR.Multiply(expression.Content); break;
                    case VariablePowExpression expression: variablesR.Add(expression); break;
                    case VariableExpression expression: variablesR.Add(expression); break;
                    case TermExpression expression:
                        {
                            coefficientR=coefficientR.Multiply(expression.Coefficient);
                            if (expression.Variables != null) variablesR.AddRange(expression.Variables);
                            if (expression.Others != null) othersR.AddRange(expression.Others);
                            break;
                        }
                    case OpMulExpression expression:
                        {
                            var result = new TermExpression(expression.Left, expression.Right);
                            coefficientR = coefficientR.Multiply(result.Coefficient);
                            variablesR.AddRange(result.Variables);
                            othersR.AddRange(result.Others);
                            break;
                        }
                    case OpDivExpression expression:
                        {
                            var result = new TermExpression(expression.Left, expression.Right.Power(NumberExpression.MinusOne));
                            coefficientR = coefficientR.Multiply(result.Coefficient);
                            variablesR.AddRange(result.Variables);
                            othersR.AddRange(result.Others);
                            break;
                        }
                    case FormulaExpression expression:
                        {
                            var result = expression.GetSingleOrDefault();
                            if (result == null) { othersR.Add(ex); break; }
                            var term = new TermExpression(result);
                            coefficientR = coefficientR.Multiply(term.Coefficient);
                            if (term.Variables != null) variablesR.AddRange(term.Variables);
                            if (term.Others != null) othersR.AddRange(term.Others);
                            break;
                        }
                    default: othersR.Add(ex);break;
                }
            }

            this.Coefficient = coefficientR;
            this.Variables = Unify(variablesR.ToArray());
            this.Others = othersR.ToArray();
        }

        public IValue Coefficient { get; }
        public VariablePowExpression[] Variables { get; } = new VariablePowExpression[0];

        public bool IsZero => Coefficient.Equals(NumberDecimal.Zero) || Others.Any(a => a.IsZero);

        public IExpression[] Others = new IExpression[0];

        public TermExpression Format() => Format(null);

        public TermExpression Format(Environment.Environment environment) => this;

        public static TermExpression AddIfCombinedable(TermExpression a, TermExpression b)
        {
            if (a.Variables.Count() != b.Variables.Count()) return null;
            if (a.Others.Count() != 0 || b.Others.Count() != 0) return null;
            for(int i = 0; i < a.Variables.Count(); i++)
            {
                if (a.Variables[i].Variable.Name != b.Variables[i].Variable.Name) return null;
                if (!a.Variables[i].Exponent.Content.Equals( b.Variables[i].Exponent.Content)) return null;
            }
            return new TermExpression(a.Coefficient.Add(b.Coefficient), a.Variables);
        }

        public IExpression[] GetMembers()
        {
            if (this.Coefficient.Equals(NumberDecimal.Zero)) return new IExpression[0];
            List<IExpression> expressions = new List<IExpression>();
            if (!this.Coefficient.Equals(NumberDecimal.One)) expressions.Add(new NumberExpression(this.Coefficient));
            expressions.AddRange(this.Variables);
            expressions.AddRange(this.Others);
            return expressions.ToArray();
        }

        public TermExpression Multiply(TermExpression value)
        {
            return new TermExpression(this, value);
        }

        public TermExpression Multiply(IValue value)
        {
            return new TermExpression(this.Coefficient.Multiply(value), this.Variables);
        }

        public TermExpression Multiply(params VariablePowExpression[] values)
        {
            return new TermExpression(this.Coefficient, Multiply(this.Variables, values).ToArray());
        }

        public static VariablePowExpression[] Unify(IEnumerable<VariablePowExpression> a)
        {
            if (a == null) return new VariablePowExpression[0];
            var result = new Dictionary<VariableExpression, IValue>();
            foreach(var item in a)
            {
                if (result.ContainsKey(item.Variable))
                {
                    result[item.Variable] = result[item.Variable].Add(item.Exponent.Content);
                }
                else
                {
                    result.Add(item.Variable, item.Exponent.Content);
                }
            }
            return result.Select(r => new VariablePowExpression(r.Key, new NumberExpression(r.Value))).OrderBy(x=>x.Variable.Name).ToArray();
        }

        public static VariablePowExpression[] Multiply(params IEnumerable<VariablePowExpression>[] vars)
        {
            return Unify(vars?.Where(a => a != null)?.SelectMany(a => a));
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression, () => new FormulaExpression(this, expression));
        }

        public TermExpression Multiply(params IExpression[] exs)
        {
            return new TermExpression(exs.Concat(new IExpression[] { this }).ToArray());
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return func(this);
        }

        public TermExpression MemberSelectVariable(Func<VariablePowExpression, VariablePowExpression> func)
        {
            return new TermExpression(this.Coefficient, this.Variables.Select(a => func(a)).ToArray(), null, this.Others);
        }

        public IExpression Power(IExpression exponent)
        {
            return Helper.ExpressionPower(this, exponent, (i) =>
            {
                return new TermExpression(this.Coefficient.Power(i),
                    this.MemberSelectVariable(a => new VariablePowExpression(a.Variable, (NumberExpression)a.Exponent.Multiply((NumberExpression)exponent))).Variables);
            }, (n) => {
                return new TermExpression(NumberDecimal.One,this.MemberSelectVariable(a => new VariablePowExpression(a.Variable, new NumberExpression(a.Exponent.Content.Multiply(n)))).Variables)
                .Multiply(new NumberExpression(this.Coefficient).Power(exponent));
            },
            () => {
                return new OpPowExpression(this, exponent);
            });
        }

        IExpression IExpression.Format()
        {
            return Format();
        }

        IExpression IExpression.Format(Environment.Environment environment)
        {
            return Format(environment);
        }

        public IExpression Expand(int PowerLevel = int.MaxValue)
        {
            var items = ExpandLevel(GetMembers().Select(a => a.Expand()).ToArray(), 0);
            IExpression result = NumberExpression.Zero;
            foreach(var item in items)
            {
                result = result.Add(item);
            }
            return result;
        }

        private IExpression[] ExpandLevel(IExpression[] items,int count)
        {
            if (items.Count() <= count) return new IExpression[] { NumberExpression.One };
            var result = new List<IExpression>();
            var splited = Helper.SplitAddition(items[count]);
            var next = ExpandLevel(items, count + 1);
            foreach (var item in splited)
            {
                foreach(var item2 in next)
                {
                    result.Add(item.Multiply(item2));
                }
            }
            return result.ToArray();
        }

        public IExpression Multiply(IExpression expression)
        {
            return new TermExpression(this, expression);
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
