using System;
using System.Collections.Generic;
using System.Text;

using kurema.Calc.Helper.Expressions;
using kurema.Calc.Helper.Interpreter;
using kurema.Calc.Helper.Values;

using System.Linq;

namespace kurema.Calc.Helper.Environment
{
    public interface IFunction
    {
        IExpression Evaluate(Environment environment, params IExpression[] expressions);
        bool CanEvaluate(int argCount);
    }

    public static class Functions
    {
        public static IFunction Reciprocal =>
            new FunctionExpressionDelegate(1, 1, (a, b) => new OpDivExpression(new NumberExpression(new NumberDecimal(1)), b[0]));
        public static IFunction Sum =>
            new FunctionExpressionDelegate(0, int.MaxValue, (a, b) => {
                return new FormulaExpression(b);
            });
        public static IFunction PrimeNext =>
            new FunctionIValueDelegate(1, 1, (a, b) =>
            {
                var cnt = b?[0]?.GetInt();
                return cnt?.Healthy == true ? new NumberExpression(new NumberDecimal(MathEx.PrimeNext(cnt.Value.Value), 0)):null;
            });
        public static IFunction Factorial =>
            new FunctionIValueDelegate(1, 1, (a, b) =>
              {
                  var cnt = b?[0]?.GetInt();
                  return cnt?.Healthy == true ? new NumberExpression(new NumberDecimal(MathEx.Factorial(cnt.Value.Value), 0)) : null;
              });

        public static IFunction EuclideanAlgorithm =>
            new FunctionIValueDelegate(2, 2, (a, b) =>
              {
                  var aval = b?[0]?.GetBigInteger();
                  var bval = b?[1]?.GetBigInteger();
                  if (!(aval?.WithinRange ?? false) || !(bval?.WithinRange ?? false)) return null;
                  return new NumberExpression(new NumberDecimal(MathEx.EuclideanAlgorithm(aval.Value.Value, bval.Value.Value),0));
              });
    }

    public class FunctionExpressionDelegate : IFunction
    {
        public readonly FunctionDelegate<IExpression> Content;

        public FunctionExpressionDelegate(int argumentCountMinimum, int argumentCountMaximum, Func<Environment, Expressions.IExpression[], IExpression> content)
        {
            Content = new FunctionDelegate<IExpression>(argumentCountMinimum, argumentCountMaximum, content, (e, a) => a);
        }

        public bool CanEvaluate(int argCount)
        {
            return Content.CanEvaluate(argCount);
        }

        public IExpression Evaluate(Environment environment, params IExpression[] expressions)
        {
            return Content.Evaluate(environment, expressions);
        }
    }

    public class FunctionIValueDelegate : IFunction
    {
        public readonly FunctionDelegate<IValue> Content;

        public FunctionIValueDelegate(int argumentCountMinimum, int argumentCountMaximum, Func<Environment, IValue[], IExpression> content)
        {
            Content = new FunctionDelegate<IValue>(argumentCountMinimum, argumentCountMaximum, content, (e, a) =>
            {
                return (a?.Format(e) as NumberExpression)?.Content;
            });
        }

        public bool CanEvaluate(int argCount)
        {
            return Content.CanEvaluate(argCount);
        }

        public IExpression Evaluate(Environment environment, params IExpression[] expressions)
        {
            return Content.Evaluate(environment, expressions);
        }
    }

    public class FunctionDelegate<T> : IFunction
    {
        public readonly int ArgumentCountMinimum;
        public readonly int ArgumentCountMaximum;
        public readonly Func<Environment, T[], IExpression> Content;
        public readonly Func<Environment, IExpression, T> Converter;

        public FunctionDelegate(int argumentCountMinimum, int argumentCountMaximum, Func<Environment, T[], IExpression> content, Func<Environment, IExpression, T> converter)
        {
            ArgumentCountMinimum = Math.Min(argumentCountMinimum, argumentCountMaximum);
            ArgumentCountMaximum = Math.Max(argumentCountMinimum, argumentCountMaximum);
            ArgumentCountMaximum = argumentCountMaximum;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public bool CanEvaluate(int argCount)
        {
            return ArgumentCountMinimum <= argCount && argCount <= ArgumentCountMaximum;
        }

        public IExpression Evaluate(Environment environment, params IExpression[] expressions)
        {
            var args = expressions.Select(a => Converter(environment, a)).ToArray();
            return Content(environment, args);
        }
    }
}
