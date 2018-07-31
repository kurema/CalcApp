using System;
using System.Collections.Generic;
using System.Text;

using kurema.Calc.Helper.Expressions;
using kurema.Calc.Helper.Interpreter;

namespace kurema.Calc.Helper
{
    public class Environment
    {
        public static string Execute(string script)
        {

            try
            {
                var lexer = new Lexer(script);
                var parser = new Parser();
                var expression = (IExpression)parser.yyparse(lexer);
                return expression.Evaluate().ToString();
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
    }
}
