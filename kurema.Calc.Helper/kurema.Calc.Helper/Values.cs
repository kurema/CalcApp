using System;
using System.Collections.Generic;
using System.Text;

using System.Numerics;

namespace kurema.Calc.Helper.Values
{
    public interface IValue
    {
        IValue Add(IValue value);
        IValue Multiply(IValue value);
        IValue Substract(IValue value);
        IValue Divide(IValue value);
        //IValue Remainder(IValue value);
    }

    public class ErrorValue : IValue
    {
        public readonly string Message;

        public ErrorValue(string message) => Message = message;

        public IValue Add(IValue value) => this;
        public IValue Divide(IValue value) => this;
        public IValue Multiply(IValue value) => this;
        public IValue Substract(IValue value) => this;

        public static class ErrorValues
        {
            public static ErrorValue UnknownValueError => new ErrorValue("Unknown value.");
            public static ErrorValue DivisionByZeroError => new ErrorValue("Division by zero error.");
        }
    }

    public class NumberRational : IValue
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
            foreach(var item in Calc.Helper.Consts.Primes.Values)
            {
                if (item  > Numerator || item  > Denominator) break;
                while(Numerator%item==0 && Denominator % item == 0)
                {
                    Numerator /= item;
                    Denominator /= item;
                }
            }
        }

        public NumberRational Add(NumberRational value) => this + value;
        public NumberRational Divide(NumberRational value) => this * value;
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
            if (a == null || b==null) return null;
            return new NumberRational(a.Numerator * b.Numerator, a.Denominator * b.Denominator);
        }

        public static IValue operator /(NumberRational a, NumberRational b)
        {
            if (a == null) return null;
            switch (b?.Reciprocal())
            {
                case ErrorValue value:return value;
                case NumberRational value:return a * value;
                case null:return null;
                default:throw new Exception("This line should not be called");
            }

        }

        public static NumberRational operator +(NumberRational a) => a;

        public static NumberRational operator -(NumberRational a) => a == null ? null : new NumberRational(-a.Numerator, a.Denominator, a.Exponent);

        public override string ToString()
        {
            return String.Format("{0} e {2} / {1}",this.Numerator,this.Denominator,this.Exponent);
        }

        #region
        public IValue Add(IValue value)
        {
            switch (value)
            {
                case NumberRational number: return Add(number);
                case NumberDecimal number: return Add(number);
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }
        public IValue Multiply(IValue value)
        {
            switch (value)
            {
                case NumberRational number: return Multiply(number);
                case NumberDecimal number: return Multiply(number);
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }
        public IValue Substract(IValue value)
        {
            switch (value)
            {
                case NumberRational number: return Substract(number);
                case NumberDecimal number: return Substract(number);
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }
        public IValue Divide(IValue value)
        {
            switch (value)
            {
                case NumberRational number: return Divide(number);
                case NumberDecimal number: return Divide(number);
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }
        #endregion
    }

    public class NumberDecimal : IValue
    {
        public readonly BigInteger Significand;
        public readonly BigInteger Exponent;

        public NumberDecimal(BigInteger significand, BigInteger exponent)
        {
            Significand = significand;
            Exponent = exponent;
        }

        public NumberDecimal(double value):this(value.ToString())
        {
        }

        public NumberDecimal(string value)
        {
            {
                var m = System.Text.RegularExpressions.Regex.Match(value, @"^([\-\+]?)(\d+)\.?(\d*)$");
                if (m.Success)
                {
                    this.Significand = BigInteger.Parse(m.Groups[2].Value + m.Groups[3].Value);
                    this.Exponent = - m.Groups[3].Length;
                    return;
                }
            }
            {
                var m = System.Text.RegularExpressions.Regex.Match(value, @"^([\-\+]?)(\d+)\.?(\d*)[eE]([\-\+]?)(\d+)$");
                if (m.Success)
                {
                    this.Significand = BigInteger.Parse(m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value);
                    this.Exponent = - m.Groups[3].Length + BigInteger.Parse(m.Groups[4].Value + m.Groups[5].Value);
                    return;
                }
            }
        }

        public NumberDecimal ShiftExponent(BigInteger exponent)
        {
            if (exponent == this.Exponent) return this;
            if (exponent < this.Exponent)
            {
                var expDiff = this.Exponent - exponent;
                if (expDiff > int.MaxValue)
                {
                    return null;
                }
                else
                {
                    var resultSig = this.Significand * BigInteger.Pow(10, (int)expDiff);
                    return new NumberDecimal(resultSig, exponent);
                }
            }
            {
                //精度が下がるので注意!
                var expDiff = exponent - this.Exponent;
                if(expDiff > int.MaxValue) { return new NumberDecimal(0,exponent); }
                var resultSig = this.Significand / BigInteger.Pow(10, (int)expDiff);
                return new NumberDecimal(resultSig, exponent);
            }
        }

        public NumberDecimal Add(NumberDecimal number) => this + number;

        public NumberDecimal Substract(NumberDecimal number) => this - number;

        public NumberDecimal Multiply(NumberDecimal number) => this * number;

        public IValue Divide(NumberDecimal number) => this / number;

        public (BigInteger,NumberDecimal,bool) DivideDecimal(NumberDecimal number)
        {
            var (a, b) = (NormalizeExponent(this, number));
            if (a == null) return (int.MaxValue, new NumberDecimal(-1,0),false);//a is too large.
            if (b == null) return (0, a, true);//b is too large
            BigInteger remainder;
            var div = BigInteger.DivRem(a.Significand, b.Significand, out remainder);
            return (div, new NumberDecimal(remainder, a.Exponent), true);
        }

        public static (NumberDecimal a,NumberDecimal b) NormalizeExponent(NumberDecimal a,NumberDecimal b)
        {
            var exp = BigInteger.Min(a.Exponent, b.Exponent);
            return (a.ShiftExponent(exp), b.ShiftExponent(exp));
        }

        public static implicit operator NumberDecimal(BigInteger value)
        {
            return new NumberDecimal(value, 0);
        }

        public static NumberDecimal operator +(NumberDecimal a, NumberDecimal b)
        {
            if (a == null || b == null) return null;
            var (ta, tb) = (NormalizeExponent(a, b));
            //指数部がint.MaxValue違う値を加算しても変化は0とみなせます。
            if (ta == null) return a;
            if (tb == null) return b;
            return new NumberDecimal(ta.Significand + tb.Significand, ta.Exponent);
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
            if (b.IsZero()) {
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
            return String.Format("{0} e {1}", this.Significand, this.Exponent);
        }

        #region
        public IValue Add(IValue value)
        {
            switch (value)
            {
                case NumberDecimal number:return Add(number);
                case NumberRational number:return ((NumberRational)this).Add(number);
                default:return ErrorValue.ErrorValues.UnknownValueError;
            }
        }
        public IValue Multiply(IValue value)
        {
            switch (value)
            {
                case NumberDecimal number: return Multiply(number);
                case NumberRational number: return ((NumberRational)this).Multiply(number);
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }
        public IValue Substract(IValue value)
        {
            switch (value)
            {
                case NumberDecimal number: return Substract(number);
                case NumberRational number: return ((NumberRational)this).Substract(number);
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }
        public IValue Divide(IValue value)
        {
            switch (value)
            {
                case NumberDecimal number: return Divide(number);
                case NumberRational number: return ((NumberRational)this).Divide(number);
                default: return ErrorValue.ErrorValues.UnknownValueError;
            }
        }
        #endregion
    }
}
