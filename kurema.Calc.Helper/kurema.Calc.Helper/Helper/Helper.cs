using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Numerics;

using kurema.Calc.Helper.Expressions;
using kurema.Calc.Helper.Values;
using System.Collections;

namespace kurema.Calc.Helper
{
    public static partial class Helper
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

        public static IExpression[] SplitAddition(IExpression expression)
        {
            switch (expression)
            {
                case OpAddExpression ex: return new IExpression[] { ex.Left, ex.Right };
                case OpSubExpression ex: return new IExpression[] { ex.Left, ex.Right.Multiply(NumberExpression.MinusOne) };
                case FormulaExpression ex: return ex.GetMembers();
                default: return new[] { expression };
            }
        }

        public static IValue GetExpressionValue(IExpression expression)
        {
            var result = expression.Format();
            if (result is NumberExpression ne) return ne.Content;
            return null;
        }

        public class PowerPermulation : IEnumerable<PowerPermulation.TermStatus>
        {
            public PowerPermulation(int members, int exponent)
            {
                Members = members;
                Exponent = exponent;
            }

            public int Members { get; }
            public int Exponent { get; }

            public IEnumerator<TermStatus> GetEnumerator()
            {
                if (Members <= 0) yield break;
                if (Members == 1)
                {
                    yield return new TermStatus(1, Exponent);
                    yield break;
                }
                if (Exponent < 0) yield break;
                var status = new int[Members - 1];
                int sum = 0;
                while (true)
                {
                    foreach(var item in new PowerPermulation(Members - 1, sum))
                    {
                        yield return TermStatus.FromStatus(item.Exponents,this.Exponent);
                    }
                    sum++;
                    if (sum > Exponent) yield break;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IExpression GetExpression(IExpression[] expressions)
            {
                if (Members != expressions.Count()) throw new ArgumentException();
                var powers = new IExpression[Members, Exponent + 1];

                for(int i = 0; i < Members; i++) 
                {
                    for(int j = 0; j < Exponent+1; j++)
                    {
                        powers[i, j] = expressions[Members - i - 1].Power(new NumberExpression(new NumberDecimal(j, 0))).Format().Expand();
                    }
                }

                IExpression result = NumberExpression.Zero;

                foreach( var item in this)
                {
                    IExpression term = new NumberExpression(new NumberDecimal(item.Coefficient, 0));
                    for(int i=0;i<item.Exponents.Count();i++)
                    {
                        term = term.Multiply(powers[i, item.Exponents[i]]).Format().Expand();
                    }
                    result = result.Add(term);
                }
                return result.Format().Expand();
            }

            public struct TermStatus
            {
                public BigInteger Coefficient;
                public int[] Exponents;

                public TermStatus(BigInteger coefficient, params int[] powers)
                {
                    Coefficient = coefficient;
                    Exponents = powers ?? throw new ArgumentNullException(nameof(powers));
                }

                public static TermStatus FromStatus(int[] status,int power)
                {
                    var result = new int[status.Length + 1];

                    BigInteger coef = 1;
                    int currentLeft = power;
                    for(int i = 0; i < status.Count(); i++)
                    {
                        coef *= MathEx.BinomialCoefficients(currentLeft, status[i]);
                        result[i] = status[i];
                        currentLeft -= status[i];
                    }
                    result[status.Length] = currentLeft;
                    return new TermStatus(coef, result);
                }
            }
        }
    }
}
