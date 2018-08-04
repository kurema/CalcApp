using System;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public class OpAddExpression:IExpression
    {
        public OpAddExpression(IExpression left, IExpression right)
        {
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
        }

        public IExpression Right { get; }

        public IExpression Left { get; }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression, (a, b) => new FormulaExpression(a.Left, a.Right, b));
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            return Left.Evaluate(environment).Add(Right.Evaluate(environment));
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            var RightF = Right.Format(environment);
            var LeftF = Left.Format(environment);

            {
                if (RightF is NumberExpression r && LeftF is NumberExpression l)
                {
                    return new NumberExpression(r.GetNumber().Add(l.GetNumber()));
                }
            }
            {
                if (RightF is FormulaExpression r)
                {
                    return r.Add(Left);
                }
            }
            {
                if (LeftF is FormulaExpression l)
                {
                    return l.Add(Right);
                }
            }
            return new FormulaExpression(Right, Left);
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new OpAddExpression(func(this.Left), func(this.Right));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression, () => this.MemberSelect(a => a.Multiply(expression)));
        }

        public override string ToString()
        {
            return Left.ToString() + "+" + Right.ToString();
        }
    }
}
