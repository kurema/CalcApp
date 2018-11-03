using System;
using System.Text;
using kurema.Calc.Helper.Environment;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public class FuncExpression : IExpression
    {
        public override string ToString()
        {
            return String.Format("{0}({1})", Name, Argument.ToString());
        }

        public FuncExpression(string name,  IExpression arg)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Argument = arg == null ? new ArgumentExpression(new IExpression[0]) :
                (arg is ArgumentExpression argument ? argument : new ArgumentExpression(new IExpression[] { arg }));
        }

        public string Name { get; }
        public ArgumentExpression Argument { get; }

        public bool IsZero => false;

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            return GetExpression(environment, (args) => new FuncExpression(this.Name, args));
        }

        public IExpression GetExpression(Environment.Environment environment, Func<ArgumentExpression, IExpression> func = null)
        {
            ArgumentExpression args = this.Argument.Format(environment);
            var f = environment.GetFunction(Name);
            if (f != null && args != null && f.CanEvaluate(args.Arguments.Length))
            {
                var result = f.Evaluate(environment, args.Arguments)?.Format(environment);
                if (result == null) return func?.Invoke(args);
                else return result;
            }
            else
            {
                return func?.Invoke(args);
            }
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression);
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression);
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new FuncExpression(this.Name, func(this.Argument));
        }

        public IExpression Power(IExpression exponent)
        {
            return Helper.ExpressionPower(this, exponent);
        }

        public IExpression Expand(int PowerLevel = int.MaxValue)
        {
            return MemberSelect(a => a.Format());
        }

        //public IExpression Differentiate(Environment.Environment environment, VariableExpression variable)
        //{
        //    var exp = GetExpression(environment);
        //    exp.
        //}
    }
}
