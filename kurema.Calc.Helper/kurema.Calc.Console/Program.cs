using System;
using System.Numerics;
namespace kurema.Calc.CUI
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.ReadLine();
            //var value = Helper.Consts.Factorials.Values[800] / Helper.Consts.Factorials.Values[700];
            //Console.WriteLine(value.ToString());
            //Console.ReadLine();


            //Console.WriteLine(Helper.Consts.Primes.Values[666]);
            //Console.WriteLine(Helper.Consts.FactorialsPrime.Values[666]);
            //Console.WriteLine(Helper.Consts.Factorials.Values[666]);

            //Console.WriteLine(Helper.Consts.Primes.Length);
            //Console.WriteLine(Helper.Consts.FactorialsPrime.Length);
            //Console.WriteLine(Helper.Consts.Factorials.Length);

            //for(int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine(Helper.MathEx.Factorial(i).ToString());
            //}

            //while (true)
            //{
            //    var a = int.Parse(Console.ReadLine());
            //    var b = int.Parse(Console.ReadLine());
            //    Console.WriteLine(String.Format("{0}P{1} ",a,b)+Helper.MathEx.FactorialPower(a, b));
            //    Console.WriteLine(String.Format("{0}C{1} ", a, b) + Helper.MathEx.BinomialCoefficients(a, b));
            //}

            while (true)
            {
                var input = Console.ReadLine();
                Console.WriteLine(kurema.Calc.Helper.Environment.Helper.Execute(input,new Helper.Environment.Environment()));
            }
        }

        static void ShowFactorialPrime(int max)
        {
            BigInteger current = 1;
            for (int i = 0; i < max; i++)
            {
                var prime = Helper.Consts.Primes.Values[i];
                current *= prime;
                ShowBigInteger(current);
            }
        }


        static void ShowFactorial(int max)
        {
            BigInteger current = 1;
            for (int i = 1; i <= max; i++)
            {
                current *= i;
                ShowBigInteger(current);
            }
        }

        static void ShowBigInteger(BigInteger a)
        {
            var value = a.ToByteArray();
            Console.Write("new BigInteger( new byte[]{ ");
            foreach (var item in value)
            {
                Console.Write(item.ToString() + ", ");
            }
            Console.WriteLine("}),");
        }
    }
}
