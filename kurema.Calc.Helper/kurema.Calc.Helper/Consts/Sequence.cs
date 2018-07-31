using System;
using System.Collections.Generic;
using System.Text;

namespace kurema.Calc.Helper
{
    public partial class Consts
    {
        public class Sequence<T>
        {
            public readonly T[] Values;
            public readonly bool ContainsFull;
            public readonly T MaxValue;
            public int Length => Values.Length;

            public Sequence(T[] values, bool containsFull, T maxValue=default(T))
            {
                Values = values ?? throw new ArgumentNullException(nameof(values));
                ContainsFull = containsFull;
                MaxValue = maxValue;
            }
        }
    }
}
