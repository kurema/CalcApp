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
        (int Value, bool Precise,bool WithinRange) GetInt();
        (BigInteger Value, bool Precise, bool WithinRange) GetBigInteger();
        IValue Remainder(IValue value);
    }
}
