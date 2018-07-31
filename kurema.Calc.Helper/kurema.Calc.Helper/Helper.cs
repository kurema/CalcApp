using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace kurema.Calc.Helper
{
    public static class Helper
    {
        public static List<int> GeneratePrime(int max)
        {
            if (max < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(max));
            }

            int sqrtMax = (int)Math.Sqrt(max);
            var isPrimes = Enumerable.Repeat(true, max - 1).ToArray();

            for (int i = 2; i < sqrtMax; ++i)
            {
                for (int j = i * 2; j <= max; j += i)
                {
                    isPrimes[j - 2] = false;
                }
            }

            return Enumerable.Range(2, max - 1).Where(x => isPrimes[x - 2]).ToList();
        }
    }
}
