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
        ConversionResult<int> GetInt();
        ConversionResult<BigInteger> GetBigInteger();
        IValue Remainder(IValue value);
    }

    public struct ConversionResult<T>
    {
        public T Value;
        public bool Precise;
        public bool WithinRange;

        public ConversionResult(T value, bool precise, bool withinRange)
        {
            Value = value;
            Precise = precise;
            WithinRange = withinRange;
        }

        public bool Healthy => Precise && WithinRange;
        public T HealthyValueOrDefault => this.Healthy ? Value : default(T);
    }
}
