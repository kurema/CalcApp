using System;
using System.Collections.Generic;
using System.Text;

using System.Numerics;

namespace kurema.Calc.Helper
{
    public static partial class MathEx
    {
        public static BigInteger Factorial(int arg)
        {
            if (arg == 0) return 1;
            if (arg < 0) return -1;
            var seq = Consts.Factorials;
            if (arg - 1 < seq.Length)
            {
                return seq.Values[arg - 1];
            }
            var current= seq.Values[seq.Length - 1];
            for (int i = seq.Length+1; i <= arg; i++)
            {
                current *= i;
            }
            return current;
        }

        public static BigInteger BinomialCoefficients(BigInteger a, BigInteger b)
        {
            if (b > int.MaxValue) return -1;
            return FactorialPower(a, b) / Factorial((int)b);
        }

        public static BigInteger FactorialPower(BigInteger a,BigInteger b)
        {
            var seq = Consts.Factorials;
            if (a < b) return 0;
            if (a == b && a < int.MaxValue) return Factorial((int)a);
            if (a >= int.MaxValue) return -1;
            if (a < 0 || b < 0) return -1;

            if (a - 1 < seq.Length)
            {
                return Factorial((int)a) / Factorial((int)(a-b));
            }
            BigInteger result = 1;
            if (a - b  < seq.Length)
            {
                result = seq.Values[seq.Length - 1] / seq.Values[(int)(a - b) - 1];
            }
            for (BigInteger i = BigInteger.Max(seq.Length + 1, a - b + 1); i <= a; i++)
            {
                result *= i;
            }
            return result;
        }
    }
}
