using System;
using System.Collections.Generic;
using System.Text;

using System.Numerics;

namespace kurema.Calc.Helper.Values
{
    public class NumberRational : IValue, IEquatable<NumberRational>, IEquatable<NumberDecimal>
    {
        public readonly BigInteger Numerator;
        public readonly BigInteger Denominator;
        public readonly BigInteger Exponent;

        public NumberRational(BigInteger numerator, BigInteger denominator, BigInteger? exponent = null)
        {
            Numerator = numerator;
            Denominator = denominator;
            Exponent = exponent ?? 1;
            if (Denominator < 0)
            {
                Denominator = BigInteger.Negate(Denominator);
                Numerator = BigInteger.Negate(Numerator);
            }
            var gcd = MathEx.EuclideanAlgorithm(this.Denominator, this.Numerator);
            this.Denominator /= gcd;
            this.Numerator /= gcd;
            while (Denominator % 2 == 0)
            {
                Denominator /= 2;
                Exponent -= 1;
                Numerator *= 5;
            }
            while (Denominator % 5 == 0)
            {
                Denominator /= 5;
                Exponent -= 1;
                Numerator *= 2;
            }
            while (Numerator % 10 == 0)
            {
                Numerator /= 10;
                Exponent++;
            }
        }

        public NumberRational Add(NumberRational value) => this + value;
        public IValue Divide(NumberRational value) => this / value;
        public NumberRational Multiply(NumberRational value) => this * value;
        public NumberRational Substract(NumberRational value) => this - value;

        public IValue Reciprocal()
        {
            if (this.Numerator == 0)
            {
                return ErrorValue.ErrorValues.DivisionByZeroError;
            }
            else
            {
                return new NumberRational(this.Denominator, this.Numerator, BigInteger.Negate(this.Exponent));
            }
        }

        public static implicit operator NumberRational(NumberDecimal value)
        {
            return new NumberRational(value.Significand, 1, value.Exponent);
        }

        public static NumberRational operator +(NumberRational a, NumberRational b)
        {
            if (a == null || b == null) return null;
            var ad = a.Denominator;
            var an = new NumberDecimal(a.Numerator, a.Exponent);
            var bd = b.Denominator;
            var bn = new NumberDecimal(b.Numerator, b.Exponent);
            var d = an.Multiply(bd).Add(bn.Multiply(ad));
            return new NumberRational(d.Significand, ad * bd, d.Exponent);
        }

        public static NumberRational operator -(NumberRational a, NumberRational b)
        {
            if (a == null || b == null) return null;
            return a + (-b);
        }

        public static NumberRational operator *(NumberRational a, NumberRational b)
        {
            if (a == null || b == null) return null;
            return new NumberRational(a.Numerator * b.Numerator, a.Denominator * b.Denominator, a.Exponent + b.Exponent);
        }

        public static IValue operator /(NumberRational a, NumberRational b)
        {
            if (a == null) return null;
            switch (b?.Reciprocal())
            {
                case ErrorValue value: return value;
                case NumberRational value: return a * value;
                case null: return null;
                default: throw new Exception("This line should not be called");
            }
        }

        public static NumberRational operator +(NumberRational a) => a;

        public static NumberRational operator -(NumberRational a) => a == null ? null : new NumberRational(-a.Numerator, a.Denominator, a.Exponent);

        //public static bool operator ==(NumberRational a, NumberRational b)
        //{
        //    if (Equals(a, null) && Equals(b, null)) return true;
        //    if (Equals(a, null) || Equals(b, null)) return false;
        //    return a.Equals(b);
        //}

        //public static bool operator !=(NumberRational a, NumberRational b)
        //{
        //    return !(a == b);
        //}

        public static IValue Remainder(NumberRational ar, IValue b)
        {
            if(ar==null || b == null)
            {
                return new ErrorValue(new NullReferenceException());
            }
            NumberRational br;
            switch (b)
            {
                case NumberRational number: br = number; break;
                case NumberDecimal number: br = number; break;
                case ErrorValue error: return error;
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
            var exp = BigInteger.Min(ar.Exponent, br.Exponent);
            var an = MathEx.ShiftExponent(ar.Numerator, ar.Exponent, exp);
            var bn = MathEx.ShiftExponent(br.Numerator, br.Exponent, exp);
            if (!bn.HasValue) return ar;
            if (!an.HasValue) return ErrorValue.ErrorValues.ExponentTooLargeError;
#if DEBUG
            System.Diagnostics.Debug.Assert(an.Precise);
            System.Diagnostics.Debug.Assert(bn.Precise);
#endif
            var lcmDen = MathEx.LeastCommonMultiple(ar.Denominator, br.Denominator);
            BigInteger.DivRem(an.Value * lcmDen / ar.Denominator, bn.Value * lcmDen / ar.Denominator, out var remainNum);

            if (lcmDen == 1) return new NumberDecimal(remainNum, exp);
            else return new NumberRational(remainNum, lcmDen,  exp);
        }

        public static IValue Power(NumberRational x, int exponent)
        {
            if (exponent == 0)
            {
                return NumberDecimal.One;
            }
            else if (exponent > 0)
            {
                return new NumberRational(
                    BigInteger.Pow(x.Numerator, exponent),
                    BigInteger.Pow(x.Denominator, exponent),
                    x.Exponent * exponent);
            }
            else
            {
                return new NumberRational(
                    BigInteger.Pow(x.Denominator, -exponent),
                    BigInteger.Pow(x.Numerator, -exponent),
                    x.Exponent * exponent);
            }
        }

        public override string ToString()
        {
            return Helper.GetString(Numerator, Denominator, Exponent);
        }

#region
        public IValue Add(IValue value)
        {
            switch (value)
            {
                case NumberRational number: return Add(number);
                case NumberDecimal number: return Add(number);
                case ErrorValue error: return error;
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }
        public IValue Multiply(IValue value)
        {
            switch (value)
            {
                case NumberRational number: return Multiply(number);
                case NumberDecimal number: return Multiply(number);
                case ErrorValue error: return error;
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }
        public IValue Substract(IValue value)
        {
            switch (value)
            {
                case NumberRational number: return Substract(number);
                case NumberDecimal number: return Substract(number);
                case ErrorValue error: return error;
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }
        public IValue Divide(IValue value)
        {
            switch (value)
            {
                case NumberRational number: return Divide(number);
                case NumberDecimal number: return Divide(number);
                case ErrorValue error: return error;
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }

        public IValue Power(int y)
        {
            return Power(this, y);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NumberRational) || Equals(obj as NumberDecimal);
        }

        public bool Equals(NumberRational other)
        {
            return !Equals(other,null) &&
                   Numerator.Equals(other.Numerator) &&
                   Denominator.Equals(other.Denominator) &&
                   Exponent.Equals(other.Exponent);
        }

        public override int GetHashCode()
        {
            var hashCode = -547078731;
            hashCode = hashCode * -1521134295 + EqualityComparer<BigInteger>.Default.GetHashCode(Numerator);
            hashCode = hashCode * -1521134295 + EqualityComparer<BigInteger>.Default.GetHashCode(Denominator);
            hashCode = hashCode * -1521134295 + EqualityComparer<BigInteger>.Default.GetHashCode(Exponent);
            return hashCode;
        }

        public bool Equals(NumberDecimal other)
        {
            return Equals(other, null) &&
                   Numerator.Equals(other.Significand) &&
                   Denominator.Equals(1) &&
                   Exponent.Equals(other.Exponent);
        }

        public bool Equals(IValue other)
        {
            return Equals(other);
        }
#endregion

        public ConversionResult<int> GetInt()
        {
            var result = GetBigInteger();

            if (MathEx.WithinIntRange(result.Value))
            {
                return new ConversionResult<int>((int)result.Value, result.Precise, result.WithinRange);
            }
            else
            {
                return new ConversionResult<int>(0, result.Precise, false);
            }
        }

        public ConversionResult<BigInteger> GetBigInteger()
        {
            if (this.Denominator == 1)
            {
                return new NumberDecimal(this.Numerator, this.Exponent).GetBigInteger();
            }
            else
            {
                var asBI = new NumberDecimal(this.Numerator, this.Exponent).GetBigInteger();
                if (!asBI.WithinRange) return new ConversionResult<BigInteger>(0, false, false);
                var result = asBI.Value / this.Denominator;
                return new ConversionResult<BigInteger>(result, false, true);
            }
        }

        public IValue Remainder(IValue value)
        {
            return Remainder(this, value);
        }
    }
}
