using System;
using System.Collections.Generic;
using System.Text;

using System.Numerics;
using System.Linq;

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

        public static int PrimeNext(int prime)
        {
            return Consts.Primes.Values.FirstOrDefault(a => a > prime);
        }

        public static bool PrimeIs(int prime)
        {
            return Consts.Primes.Values.Contains(prime);
        }

        public static BigInteger EuclideanAlgorithm(BigInteger a,BigInteger b)
        {
            if (a < b)
            {
                var c = a;
                a = b;
                b = c;
            }
            while (true)
            {
                BigInteger c;
                BigInteger.DivRem(a, b, out c);
                if (c == 0) return b;
                a = c;
                BigInteger.DivRem(b, a, out c);
                if (c == 0) return a;
                b = c;
            }
        }

        public static BigInteger LeastCommonMultiple(BigInteger a, BigInteger b)
        {
            return a * b / EuclideanAlgorithm(a, b);
        }

        public static bool WithinIntRange(BigInteger? integer)
        {
            return integer <= int.MaxValue && integer >= int.MinValue;
        }

        public static ShiftResult ShiftExponent(BigInteger Significand,BigInteger Exponent,  BigInteger exponent)
        {
            if (exponent == Exponent) return new ShiftResult(Significand, true, true);
            if (exponent < Exponent)
            {
                var expDiff = Exponent - exponent;
                if (expDiff > int.MaxValue)
                {
                    return new ShiftResult(0, false, false);
                }
                else
                {
                    var resultSig = Significand * BigInteger.Pow(10, (int)expDiff);
                    return new ShiftResult(resultSig, true, true);
                }
            }
            {
                var expDiff = exponent - Exponent;
                if (expDiff > int.MaxValue) { return new ShiftResult(0, true, false); }
                var resultSig = Significand / BigInteger.Pow(10, (int)expDiff);
                return new ShiftResult(resultSig, true, false);
            }
        }

        public struct ShiftResult
        {
            public BigInteger Value;
            public bool HasValue;
            public bool Precise;

            public ShiftResult(BigInteger value, bool hasValue, bool precise)
            {
                Value = value;
                HasValue = hasValue;
                Precise = precise;
            }

            public BigInteger? GetNullable()
            {
                if (HasValue) return Value;
                else return null;
            }
        }
    }
}
