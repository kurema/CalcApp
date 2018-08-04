using System;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public class VariableExpression : IExpression
    {
        public override string ToString()
        {
            return Variable?.ToString();
        }

        public string Variable { get; }

        public VariableExpression(string variable)
        {
            this.Variable = variable ?? throw new ArgumentNullException(nameof(variable));
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            var ex = environment.GetVariable(Variable);
            if (ex == null) return new NumberDecimal(0);
            return ex.Evaluate(environment);
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            if (environment == null) return this;
            var ex = environment.GetVariable(Variable);
            if (ex == null) return this;
            return ex;
        }

        public static bool operator ==(VariableExpression a, VariableExpression b)
        {
            return a?.Variable == b?.Variable;
        }

        public static bool operator !=(VariableExpression a, VariableExpression b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Variable.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VariableExpression);
        }

        public IExpression Add(IExpression expression)
        {
            throw new NotImplementedException();
        }

        public IExpression Multiply(IExpression expression)
        {
            throw new NotImplementedException();
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            throw new NotImplementedException();
        }
    }
}
