using System;
using System.Collections.Generic;
using System.Text;

using System.Numerics;

namespace kurema.Calc.Helper.Values
{
    public class NumberDecimal : IValue, IEquatable<NumberRational>, IEquatable<NumberDecimal>
    {
        public readonly BigInteger Significand;
        public readonly BigInteger Exponent;

        public static NumberDecimal Zero => new NumberDecimal(0, 0);
        public static NumberDecimal One => new NumberDecimal(1, 0);
        public static NumberDecimal MinusOne => new NumberDecimal(-1, 0);

        public NumberDecimal(BigInteger significand, BigInteger exponent)
        {
            (Significand, Exponent) = FixExponent(significand, exponent);
        }

        public static (BigInteger significand, BigInteger exponent) FixExponent(BigInteger significand, BigInteger exponent)
        {
            if (significand == 0) return (0, 0);
            while (significand % 10 == 0)
            {
                significand /= 10;
                exponent++;
            }
            return (significand, exponent);
        }

        public NumberDecimal(double value) : this(value.ToString())
        {
        }

        public NumberDecimal(string value)
        {
            {
                var m = System.Text.RegularExpressions.Regex.Match(value, @"^([\-\+]?)(\d+)\.?(\d*)$");
                if (m.Success)
                {
                    this.Significand = BigInteger.Parse(m.Groups[2].Value + m.Groups[3].Value);
                    this.Exponent = -m.Groups[3].Length;
                    (Significand, Exponent) = FixExponent(Significand, Exponent);
                    return;
                }
            }
            {
                var m = System.Text.RegularExpressions.Regex.Match(value, @"^([\-\+]?)(\d+)\.?(\d*)[eE]([\-\+]?)(\d+)$");
                if (m.Success)
                {
                    this.Significand = BigInteger.Parse(m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value);
                    this.Exponent = -m.Groups[3].Length + BigInteger.Parse(m.Groups[4].Value + m.Groups[5].Value);
                    (Significand, Exponent) = FixExponent(Significand, Exponent);
                    return;
                }
            }
            throw new Exception("Failed to Parse.");
        }

        public MathEx.ShiftResult ShiftExponent(BigInteger exponent)
        {
            return MathEx.ShiftExponent(this.Significand, this.Exponent, exponent);
        }

        public NumberDecimal Add(NumberDecimal number) => this + number;

        public NumberDecimal Substract(NumberDecimal number) => this - number;

        public NumberDecimal Multiply(NumberDecimal number) => this * number;

        public IValue Divide(NumberDecimal number) => this / number;

        public (BigInteger, NumberDecimal, bool) DivideDecimal(NumberDecimal number)
        {
            var (a, b, e) = (NormalizeExponent(this, number));
            if (!a.HasValue) return (int.MaxValue, new NumberDecimal(-1, 0), false);//a is too large.
            if (!b.HasValue) return (0, this, true);//b is too large
            var div = BigInteger.DivRem(a.Value, b.Value, out BigInteger remainder);
            return (div, new NumberDecimal(remainder, e), true);
        }

        public static (BigInteger? a, BigInteger? b, BigInteger exponent) NormalizeExponent(NumberDecimal a, NumberDecimal b)
        {
            var exp = BigInteger.Min(a.Exponent, b.Exponent);
            return (a.ShiftExponent(exp).GetNullable(), b.ShiftExponent(exp).GetNullable(), exp);
        }

        public static implicit operator NumberDecimal(BigInteger value)
        {
            return new NumberDecimal(value, 0);
        }

        public static NumberDecimal operator +(NumberDecimal a, NumberDecimal b)
        {
            if (a == null || b == null) return null;
            var (ta, tb, e) = (NormalizeExponent(a, b));
            //指数部がint.MaxValue違う値を加算しても変化は0とみなせます。
            if (ta == null) return a;
            if (tb == null) return b;
            return new NumberDecimal(ta.Value + tb.Value, e);
        }
        public static NumberDecimal operator -(NumberDecimal a, NumberDecimal b) => a + (-b);

        public static NumberDecimal operator *(NumberDecimal a, NumberDecimal b)
        {
            if (a == null || b == null) return null;
            return new NumberDecimal(a.Significand * b.Significand, a.Exponent + b.Exponent);
        }
        public static IValue operator /(NumberDecimal a, NumberDecimal b)
        {
            if (a == null || b == null) return null;
            if (b.IsZero())
            {
                return ErrorValue.ErrorValues.DivisionByZeroError;
            }
            else
            {
                return new NumberRational(a.Significand, b.Significand, a.Exponent - b.Exponent);
            }
        }

        public static NumberDecimal operator +(NumberDecimal a) => a;
        public static NumberDecimal operator -(NumberDecimal a) => a == null ? null : new NumberDecimal(-a.Significand, a.Exponent);

        public bool IsZero()
        {
            return this.Significand == 0;
        }

        public override string ToString()
        {
            return Helper.GetString(Significand, 1, Exponent);
        }

        public static IValue Power(NumberDecimal x, int exponent)
        {
            if (exponent > 0)
            {
                return new NumberDecimal(
                    BigInteger.Pow(x.Significand, exponent),
                    x.Exponent * exponent);
            }else if (exponent == 0)
            {
                return One;
            }
            else
            {
                return new NumberRational(1, BigInteger.Pow(x.Significand, -exponent), x.Exponent * exponent);
            }
        }

        public ConversionResult<int> GetInt()
        {
            var target = this.ShiftExponent(0);
            bool precise = (this.Exponent >= 0);
            if (MathEx.WithinIntRange(target.GetNullable()))
            {
                return new ConversionResult<int>((int)target.Value, precise, true);
            }
            else
            {
                return new ConversionResult<int>(0, precise, false);
            }
        }

        public ConversionResult<BigInteger> GetBigInteger()
        {
            var target = this.ShiftExponent(0);
            bool precise = (this.Exponent >= 0);
            if (target.HasValue)
                return new ConversionResult<BigInteger>(target.Value, precise, true);
            else
                return new ConversionResult<BigInteger>(0, precise, false);
        }


        #region
        public IValue Add(IValue value)
        {
            switch (value)
            {
                case NumberDecimal number: return Add(number);
                case NumberRational number: return ((NumberRational)this).Add(number);
                case ErrorValue error:return error;
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }
        public IValue Multiply(IValue value)
        {
            switch (value)
            {
                case NumberDecimal number: return Multiply(number);
                case NumberRational number: return ((NumberRational)this).Multiply(number);
                case ErrorValue error: return error;
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }
        public IValue Substract(IValue value)
        {
            switch (value)
            {
                case NumberDecimal number: return Substract(number);
                case NumberRational number: return ((NumberRational)this).Substract(number);
                case ErrorValue error: return error;
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }
        public IValue Divide(IValue value)
        {
            switch (value)
            {
                case NumberDecimal number: return Divide(number);
                case NumberRational number: return ((NumberRational)this).Divide(number);
                case ErrorValue error: return error;
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }

        public IValue Power(int y)
        {
            return Power(this, y);
        }

        public bool Equals(NumberDecimal other)
        {
            return !Equals(other,null) &&
                this.Significand.Equals(other.Significand) &&
                this.Exponent.Equals(other.Exponent);
        }

        public bool Equals(NumberRational other)
        {
            return other?.Equals(this) ?? false;
        }

        public bool Equals(IValue other)
        {
            return this.Equals(other as NumberDecimal) || this.Equals(other as NumberRational);
        }

        public IValue Remainder(IValue value)
        {
            return NumberRational.Remainder(this, value);
        }
        #endregion
    }
}
