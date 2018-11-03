using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Numerics;

using kurema.Calc.Helper.Expressions;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper
{
    public static partial class Helper
    {
        public static IExpression ExpressionMul<T>(T a, IExpression b) where T : IExpression
        {
            return ExpressionMul(a, b, () => new OpMulExpression(a, b));
        }

        public static IExpression ExpressionMul<T>(T a, IExpression b, Func<IExpression> func) where T : IExpression
        {
            return ExpressionMul(a, b, (c, d) => func());
        }


        public static IExpression ExpressionMul<T>(T a, IExpression b, Func<T, IExpression, IExpression> func) where T : IExpression
        {
            if (a == null || b == null) return null;
            if (a.IsZero || b.IsZero) return NumberExpression.Zero;
            { if (b is NumberExpression number && number.Content.Equals(NumberDecimal.One)) return a; }
            return func(a, b);
        }

        public static IExpression ExpressionAdd<T>(T a, IExpression b) where T : IExpression
        {
            return ExpressionAdd(a, b, () => new FormulaExpression(a, b));
        }

        public static IExpression ExpressionAdd<T>(T a, IExpression b, Func<IExpression> func) where T : IExpression
        {
            return ExpressionAdd(a, b, (c, d) => func());
        }

        public static IExpression ExpressionAdd<T>(T a, IExpression b, Func<T, IExpression, IExpression> func) where T : IExpression
        {
            if (a == null || b == null) return null;
            if (a.IsZero && b.IsZero) return NumberExpression.Zero;
            if (a.IsZero) return b;
            if (b.IsZero) return a;
            return func(a, b);
        }

        public static IExpression ExpressionPower<T>(T a, IExpression b) where T : IExpression => ExpressionPower(a, b, () => new OpPowExpression(a, b));

        public static IExpression ExpressionPower<T>(T a, IExpression b, Func<IExpression> CaseOther) where T : IExpression => ExpressionPowerIValue(a, b, (_) => CaseOther(), CaseOther);

        public static IExpression ExpressionPower<T>(T a, IExpression b, Func<int, IExpression> CaseInt, Func<IExpression> CaseOther) where T : IExpression => ExpressionPower(a, b, CaseInt, (_) => CaseOther(), CaseOther);

        public static IExpression ExpressionPower<T>(T a,IExpression b,Func<int,IExpression> CaseInt,Func<IValue,IExpression> CaseValue,Func<IExpression> CaseOther) where T : IExpression
        {
            return ExpressionPowerIValue(a, b, (v) =>
              {
                  var nint = v.GetInt();
                  if (nint.WithinRange && nint.Precise)
                  {
                      return CaseInt(nint.Value);
                  }
                  else
                  {
                      return CaseValue(v);
                  }
              }, CaseOther);
        }

        private static IExpression ExpressionPowerIValue<T>(T a, IExpression b, Func<IValue, IExpression> CaseValue, Func<IExpression> CaseOther) where T : IExpression
        {
            if(b.IsZero)return NumberExpression.One;
            if (b is NumberExpression num)
            {
                if (num.Content.Equals(NumberDecimal.One)) return a;
                //if (num.Content.Equals(NumberDecimal.Zero)) return NumberExpression.One;
                return CaseValue(num.Content);
            }
            else
            {
                return CaseOther();
            }
        }
    }
}
