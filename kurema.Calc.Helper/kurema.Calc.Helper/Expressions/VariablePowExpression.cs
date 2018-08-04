using System;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public class VariablePowExpression : IExpression
    {
        public override string ToString()
        {
            if (Exponent.Content.Equals(NumberDecimal.One)) return Variable.ToString();
            return Variable.ToString() + "^" + Exponent.ToString();
        }

        public VariablePowExpression(VariableExpression variable, NumberExpression exponent = null)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            Exponent = exponent ?? new NumberExpression(NumberDecimal.One);
        }

        public VariableExpression Variable { get; }
        public NumberExpression Exponent { get; }

        public IValue Evaluate(Environment.Environment environment)
        {
            (int Value, bool Precise, bool WithinRange) a;
            if (Exponent.Content is NumberDecimal n)
            {
                a = n.GetInt();
            }
            else if (Exponent.Content is NumberRational m)
            {
                a = m.GetInt();
            }
            else
            {
                throw new NotImplementedException();
            }
            if (a.WithinRange)
            {
                return Variable.Evaluate(environment).Power(a.Value);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            return this;
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd((TermExpression)this, expression);
        }

        public IExpression Multiply(IExpression expressionBase)
        {
            return Helper.ExpressionMul(this, expressionBase, () =>
              {
                  switch (expressionBase)
                  {
                      case VariableExpression expression when expression==this.Variable:return new VariablePowExpression(this.Variable, this.Exponent.Add(NumberDecimal.One));
                      case FormulaExpression expression:return expression.Multiply(this);
                      case TermExpression expression:return expression.Multiply(this);
                      default:return new TermExpression(null, this).Multiply(expressionBase);
                  }
              });
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return func(this);
        }

        public VariablePowExpression Power(NumberExpression exponent)
        {
            return new VariablePowExpression(this.Variable, (NumberExpression)this.Exponent.Multiply(exponent));
        }

        public IExpression Power(IExpression exponent)
        {
            return Helper.ExpressionPower(this, exponent,
                (_) => new VariablePowExpression(this.Variable, (NumberExpression)this.Exponent.Add((NumberExpression)exponent)),
                (_) => new VariablePowExpression(this.Variable, (NumberExpression)this.Exponent.Add((NumberExpression)exponent)),
                () => new OpPowExpression(this, exponent));
        }

        public static implicit operator VariablePowExpression(VariableExpression value)
        {
            return new VariablePowExpression(value);
        }
    }
}
