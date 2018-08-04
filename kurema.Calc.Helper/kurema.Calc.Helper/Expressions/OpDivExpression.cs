using System;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Expressions
{
    public class OpDivExpression : IExpression
    {
        public override string ToString()
        {
            return Left.ToString() + "/" + Right.ToString();
        }

        public OpDivExpression(IExpression left, IExpression right)
        {
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
        }

        public IExpression Right { get; }

        public IExpression Left { get; }

        public IValue Evaluate(Environment.Environment environment)
        {
            return Left.Evaluate(environment).Divide(Right.Evaluate(environment));
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            var leftF = Left.Format();
            var rightF = Right.Format();

            {
                if (leftF is NumberExpression leftN && rightF is NumberExpression rightN)
                {
                    return new NumberExpression(leftN.Content.Divide(rightN.Content));
                }
            }
            //Powerと間違えた。
            //{
            //    if (leftF is VariablePowExpression leftN && rightF is NumberExpression rightN)
            //    {
            //        return new VariablePowExpression(leftN.Variable, new NumberExpression(leftN.Exponent.Content.Add(rightN.Content)));
            //    }
            //}
            //{
            //    if (leftF is VariableExpression leftN && rightF is NumberExpression rightN)
            //    {
            //        return new VariablePowExpression(leftN, rightN);
            //    }
            //}

            return new OpDivExpression(Left.Format(), Right.Format());
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
            return new OpDivExpression(func(Left), func(Right));
        }
    }
}
