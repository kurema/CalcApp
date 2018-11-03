using System;
using System.Collections.Generic;
using System.Text;

using System.Numerics;

namespace kurema.Calc.Helper.Values
{
    public class ErrorValue : IValue
    {
        public readonly string Message;
        public readonly Exception Exception;

        public ErrorValue(string message) => Message = message;
        public ErrorValue(Exception exception)
        {
            this.Message = exception.Message;
            this.Exception = exception;
        }

        public IValue Add(IValue value) => this;
        public IValue Divide(IValue value) => this;
        public IValue Multiply(IValue value) => this;
        public IValue Power(int y) => this;
        public IValue Remainder(IValue value) => this;
        public IValue Substract(IValue value) => this;

        public bool Equals(IValue other)
        {
            if (other is ErrorValue e) return this.Message == e.Message;
            return false;
        }

        public ConversionResult<int> GetInt() =>new ConversionResult<int>(0, false, false);

        public ConversionResult<BigInteger> GetBigInteger() => new ConversionResult<BigInteger>(0, false, false);


        public static class ErrorValues
        {
            public static ErrorValue UnknownValueError => new ErrorValue("Unknown value.");
            public static ErrorValue DivisionByZeroError => new ErrorValue("Division by zero error.");
            public static ErrorValue ExponentTooLargeError => new ErrorValue("Exponent is too large.");
        }

        public override string ToString()
        {
            return Message.ToString();
        }
    }
}
