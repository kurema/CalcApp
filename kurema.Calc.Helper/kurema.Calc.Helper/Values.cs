using System;
using System.Collections.Generic;
using System.Text;

using System.Numerics;

namespace kurema.Calc.Helper.Values
{
    public interface IValue: IEquatable<IValue>
    {
        IValue Add(IValue value);
        IValue Multiply(IValue value);
        IValue Substract(IValue value);
        IValue Divide(IValue value);
        IValue Power(int y);
        //IValue Remainder(IValue value);
    }

    public class ErrorValue : IValue
    {
        public readonly string Message;

        public ErrorValue(string message) => Message = message;

        public IValue Add(IValue value) => this;
        public IValue Divide(IValue value) => this;
        public IValue Multiply(IValue value) => this;
        public IValue Power(double y) => this;

        public IValue Power(int y)
        {
            throw new NotImplementedException();
        }

        public IValue Substract(IValue value) => this;

        public bool Equals(IValue other)
        {
            if (other is ErrorValue e) return this.Message == e.Message;
            return false;
        }

        public static class ErrorValues
        {
            public static ErrorValue UnknownValueError => new ErrorValue("Unknown value.");
            public static ErrorValue DivisionByZeroError => new ErrorValue("Division by zero error.");
        }
    }

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
            foreach(var item in Calc.Helper.Consts.Primes.Values)
            {
                if (item  > Numerator || item  > Denominator) break;
                while(Numerator%item==0 && Denominator % item == 0)
                {
                    Numerator /= item;
                    Denominator /= item;
                }
            }
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
            if (a == null || b==null) return null;
            return new NumberRational(a.Numerator * b.Numerator, a.Denominator * b.Denominator, a.Exponent + b.Exponent);
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

        public static bool operator ==(NumberRational a, NumberRational b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(NumberRational a, NumberRational b)
        {
            return !(a == b);
        }


        public static NumberRational Power(NumberRational x,int exponent)
        {
            return new NumberRational(
                BigInteger.Pow(x.Numerator, exponent),
                BigInteger.Pow(x.Denominator, exponent),
                x.Exponent * exponent);
        }

        public int? GetInt()
        {
            if (this.Denominator != 1) return null;
            return new NumberDecimal(this.Numerator, this.Exponent).GetInt();
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

        public IValue Power(int y)
        {
            return Power(y);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NumberRational) || Equals(obj as NumberDecimal);
        }

        public bool Equals(NumberRational other)
        {
            return other != null &&
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
            return other != null &&
                   Numerator.Equals(other.Significand) &&
                   Denominator.Equals(1) &&
                   Exponent.Equals(other.Exponent);
        }

        public bool Equals(IValue other)
        {
            return Equals(other);
        }
        #endregion
    }

    public class NumberDecimal : IValue, IEquatable<NumberRational>, IEquatable<NumberDecimal>
    {
        public readonly BigInteger Significand;
        public readonly BigInteger Exponent;

        public static NumberDecimal Zero => new NumberDecimal(0, 0);
        public static NumberDecimal One => new NumberDecimal(1, 0);

        public NumberDecimal(BigInteger significand, BigInteger exponent)
        {
            (Significand , Exponent) = FixExponent(significand, exponent);
        }

        public static (BigInteger significand, BigInteger exponent) FixExponent(BigInteger significand, BigInteger exponent)
        {
            if(significand==0) return (significand, exponent);
            while (significand % 10 == 0)
            {
                significand /= 10;
                exponent++;
            }
            return (significand, exponent);
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
                    (Significand, Exponent) = FixExponent(Significand, Exponent);
                    return;
                }
            }
            {
                var m = System.Text.RegularExpressions.Regex.Match(value, @"^([\-\+]?)(\d+)\.?(\d*)[eE]([\-\+]?)(\d+)$");
                if (m.Success)
                {
                    this.Significand = BigInteger.Parse(m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value);
                    this.Exponent = - m.Groups[3].Length + BigInteger.Parse(m.Groups[4].Value + m.Groups[5].Value);
                    (Significand, Exponent) = FixExponent(Significand, Exponent);
                    return;
                }
            }
            throw new Exception("Failed to Parse.");
        }

        public BigInteger? ShiftExponent(BigInteger exponent)
        {
            if (exponent == this.Exponent) return this.Significand;
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
                    return resultSig;
                }
            }
            {
                //精度が下がるので注意!
                var expDiff = exponent - this.Exponent;
                if(expDiff > int.MaxValue) { return 0; }
                var resultSig = this.Significand / BigInteger.Pow(10, (int)expDiff);
                return resultSig;
            }
        }

        public NumberDecimal Add(NumberDecimal number) => this + number;

        public NumberDecimal Substract(NumberDecimal number) => this - number;

        public NumberDecimal Multiply(NumberDecimal number) => this * number;

        public IValue Divide(NumberDecimal number) => this / number;

        public (BigInteger,NumberDecimal,bool) DivideDecimal(NumberDecimal number)
        {
            var (a, b, e) = (NormalizeExponent(this, number));
            if (!a.HasValue) return (int.MaxValue, new NumberDecimal(-1,0),false);//a is too large.
            if (!b.HasValue) return (0, this, true);//b is too large
            var div = BigInteger.DivRem(a.Value, b.Value, out BigInteger remainder);
            return (div, new NumberDecimal(remainder, e), true);
        }

        public static (BigInteger? a,BigInteger? b,BigInteger exponent) NormalizeExponent(NumberDecimal a,NumberDecimal b)
        {
            var exp = BigInteger.Min(a.Exponent, b.Exponent);
            return (a.ShiftExponent(exp), b.ShiftExponent(exp),exp);
        }

        public static implicit operator NumberDecimal(BigInteger value)
        {
            return new NumberDecimal(value, 0);
        }

        public static NumberDecimal operator +(NumberDecimal a, NumberDecimal b)
        {
            if (a == null || b == null) return null;
            var (ta, tb,e) = (NormalizeExponent(a, b));
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
            return Helper.GetString(Significand, 1, Exponent);
        }

        public static NumberDecimal Power(NumberDecimal x, int exponent)
        {
            return new NumberDecimal(
                BigInteger.Pow(x.Significand, exponent),
                x.Exponent * exponent);
        }

        public int? GetInt()
        {
            if (this.Exponent < 0) return null;
            var target = this.ShiftExponent(0);
            if (target >= Int32.MinValue && target <= Int32.MaxValue)
            {
                return (int)target;
            }
            return null;
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

        public IValue Power(int y)
        {
            return Power(this, y);
        }

        public bool Equals(NumberDecimal other)
        {
            return other != null &&
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
        #endregion
    }
}
