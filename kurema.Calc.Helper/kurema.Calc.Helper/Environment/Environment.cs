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
                var lexer = new Lexer(script,environment);
                var parser = new Parser();
                var expression = (IExpression)parser.yyparse(lexer);
                var formated = expression.Format(environment);
                return formated.ToString();
                //return expression.Evaluate(environment).ToString();
            }
            catch (Exception e)
            {
#if DEBUG
                return e.ToString();
#else
                return e.Message;
#endif
            }
        }
    }

    public class Environment
    {
        public Dictionary<string, IFunction> Functions = new Dictionary<string, IFunction>() {
            {"rec",Calc.Helper.Environment.Functions.Reciprocal },
            {"sum",Calc.Helper.Environment.Functions.Sum },
            {"prime",Calc.Helper.Environment.Functions.PrimeNext },
            {"fact",Calc.Helper.Environment.Functions.Factorial },
            { "gcd",Calc.Helper.Environment.Functions.EuclideanAlgorithm},
        };

        public Dictionary<string, IExpression> Variables = new Dictionary<string, IExpression>()
        {
            {"pi",new NumberExpression( new NumberDecimal(Math.PI)) }
        };

        public IFunction GetFunction(string name)
        {
            if (Functions.TryGetValue(name, out IFunction result)) return result; else return null;
        }

        public IExpression GetVariable(string name)
        {
            if (Variables.TryGetValue(name, out IExpression result)) return result; else return null;
        }
    }

}
