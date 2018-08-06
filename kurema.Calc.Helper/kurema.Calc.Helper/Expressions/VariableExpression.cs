using System;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public class VariableExpression : IExpression
    {
        public override string ToString()
        {
            return Name?.ToString();
        }

        public string Name { get; }

        public bool IsZero => false;

        public VariableExpression(string variable)
        {
            this.Name = variable ?? throw new ArgumentNullException(nameof(variable));
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            if (environment == null) return this;
            var ex = environment.GetVariable(Name);
            if (ex == null) return this;
            return ex;
        }

        public static bool operator ==(VariableExpression a, VariableExpression b)
        {
            return a?.Name == b?.Name;
        }

        public static bool operator !=(VariableExpression a, VariableExpression b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this == (obj as VariableExpression);
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression);
        }

        public IExpression Multiply(IExpression expression)
        {
            return ((VariablePowExpression)this).Multiply(expression);
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return func(this);
        }

        public IExpression Power(IExpression exponent)
        {
            return Helper.ExpressionPower(this, exponent, (_) => new VariablePowExpression(this, (NumberExpression)exponent), (_) => new VariablePowExpression(this, (NumberExpression)exponent), () => new OpPowExpression(this, exponent));
        }

        public IExpression Expand(int PowerLevel = int.MaxValue)
        {
            return this;
        }
    }
}
