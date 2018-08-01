using System;
using System.Collections.Generic;
using System.Text;

using kurema.Calc.Helper.Expressions;
using kurema.Calc.Helper.Interpreter;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Environment
{
    public static class Helper
    {
        public static string Execute(string script,Environment environment)
        {
            try
            {
                var lexer = new Lexer(script);
                var parser = new Parser();
                var expression = (IExpression)parser.yyparse(lexer);
                return expression.Evaluate(environment).ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }

    public class Environment
    {
        public Dictionary<string, IFunction> Functions = new Dictionary<string, IFunction>() {
            {"rec",Calc.Helper.Environment.Functions.Reciprocal },
            {"sum",Calc.Helper.Environment.Functions.Sum },
            {"prime",Calc.Helper.Environment.Functions.Prime },
        };
    }

    public interface IFunction
    {
        IExpression Evaluate(Environment environment, params IExpression[] expressions);
        bool CanEvaluate(int argCount);
    }

    public static class Functions
    {
        public static IFunction Reciprocal =>
            new FunctionDelegate(1, 1, (a, b) => new OpDivExpression(new NumberExpression(new NumberDecimal(1)), b[0]), "rec");
        public static IFunction Sum =>
            new FunctionDelegate(0, int.MaxValue, (a, b) => {
                if (b.Length == 0) return new NumberExpression(new NumberDecimal(0));
                var result = b[0];
                for(int i = 1; i < b.Length; i++)
                {
                    result = new OpAddExpression(result, b[i]);
                }
                return result;
            },"sum");
        public static IFunction Prime =>
            new FunctionDelegate(1, 1, (a, b) =>
            {
                var c = b[0].Evaluate(a);
                if(c is NumberDecimal d)
                {
                    //Fix me.
                    if( d.Significand>=0 && d.Significand < Consts.Primes.Length)
                    {
                        return new NumberExpression(new NumberDecimal(Consts.Primes.Values[(int)d.Significand], 1));
                    } 
                }
                throw new Exception("Failed");
            }, "prime");
    }

    public class FunctionDelegate:IFunction
    {
        public readonly int ArgumentCountMinimum;
        public readonly int ArgumentCountMaximum;
        public readonly Func<Environment, Expressions.IExpression[], IExpression> Content;
        public readonly string DefaultName;

        public FunctionDelegate(int argumentCountMinimum, int argumentCountMaximum, Func<Environment,Expressions.IExpression[], IExpression> content, string name)
        {
            ArgumentCountMinimum = Math.Min( argumentCountMinimum,argumentCountMaximum);
            ArgumentCountMaximum = Math.Max(argumentCountMinimum, argumentCountMaximum);
            Content = content ?? throw new ArgumentNullException(nameof(content));
            DefaultName = name ?? throw new ArgumentNullException(nameof(name));
        }

        public bool CanEvaluate(int argCount)
        {
            return ArgumentCountMinimum <= argCount && argCount < ArgumentCountMaximum;
        }

        public IExpression Evaluate(Environment environment, params IExpression[] expressions)
        {
            return Content(environment, expressions);
        }
    }
}
