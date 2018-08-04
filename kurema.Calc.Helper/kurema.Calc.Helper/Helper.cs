using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Numerics;

using kurema.Calc.Helper.Expressions;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper
{
    public static class Helper
    {
        public static List<int> GeneratePrime(int max)
        {
            if (max < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(max));
            }

            int sqrtMax = (int)Math.Sqrt(max);
            var isPrimes = Enumerable.Repeat(true, max - 1).ToArray();

            for (int i = 2; i < sqrtMax; ++i)
            {
                for (int j = i * 2; j <= max; j += i)
                {
                    isPrimes[j - 2] = false;
                }
            }

            return Enumerable.Range(2, max - 1).Where(x => isPrimes[x - 2]).ToList();
        }

        public static string GetString(BigInteger Numerator,BigInteger Denominator,BigInteger Exponent)
        {
            if (Numerator == 0) return "0";
            var num = Numerator.ToString();
            if (Exponent == 0)
            {
            }
            else if (Exponent < 10 && Exponent > 0)
            {
                num = num + new string('0', (int)Exponent);
            }
            else if (-Exponent < num.Length && Exponent < 0)
            {
                var dec= num.Substring(num.Length + (int)Exponent).TrimEnd('0');
                num = num.Substring(0, num.Length + (int)Exponent) + (dec.Length > 0 ? "." + dec : "");
            }
            else if (-Exponent - num.Length < 10 && Exponent < 0)
            {
                var dec = new string('0', -(int)Exponent - num.Length) + num.TrimEnd('0');
                num = "0" + (dec.Length > 0 ? "." + dec : "");
            }
            else
            {
                num = num[0] + (num.Length > 1 ? "." + num.Substring(1).TrimEnd('0') : "") + "e" + (Exponent + (num.Length - 1)).ToString();
            }
            var builder = new StringBuilder(num);
            if (Denominator == 0)
            {
                return Values.ErrorValue.ErrorValues.DivisionByZeroError.Message;
            }else if (Denominator != 1)
            {
                builder.Append("/");
                builder.Append(Denominator.ToString());
            }
            return builder.ToString();
        }

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
            { if (b is NumberExpression number && number.Content == NumberDecimal.Zero) return NumberExpression.Zero; }
            { if (b is NumberExpression number && number.Content == NumberDecimal.One) return a; }
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
            { if (b is NumberExpression number && number.Content == NumberDecimal.Zero) return a; }
            return func(a, b);
        }

    }
}
