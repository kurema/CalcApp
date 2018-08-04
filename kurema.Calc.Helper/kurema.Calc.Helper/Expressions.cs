using System;
using System.Collections.Generic;
using System.Text;
using kurema.Calc.Helper.Environment;
using kurema.Calc.Helper.Values;

using System.Linq;

namespace kurema.Calc.Helper.Expressions
{
    public interface IExpression
    {
        IValue Evaluate(Environment.Environment environment);
        IExpression Format();
        IExpression Format(Environment.Environment environment);

        IExpression Add(IExpression expression);
        IExpression Multiply(IExpression expression);

        IExpression MemberSelect(Func<IExpression, IExpression> func);
    }

    public class NumberExpression : IExpression
    {
        public NumberExpression(IValue content)
        {
            this.Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public IValue Content { get; }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression, () =>
            {
                switch (expression)
                {
                    case NumberExpression number: return new NumberExpression(this.Content.Add(number.Content));
                    case ArgumentExpression argument: return argument.MemberSelect(a => a.Add(this));
                    case FormulaExpression formula: return formula.Add(this);
                    default:
                        return new FormulaExpression(expression, this);
                }
            });
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            return Content;
        }

        public IExpression Format()
        {
            return this;
        }

        public IExpression Format(Environment.Environment environment) => Format();

        public IValue GetNumber()
        {
            return Content;
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return func(this);
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression, () =>
            {
                switch (expression)
                {
                    case NumberExpression number: return new NumberExpression(this.Content.Multiply(number.Content));
                    case ArgumentExpression argument: return argument.MemberSelect(a => a.Multiply(this));
                    case FormulaExpression formula: return formula.Multiply(this);
                    default:
                        return new FormulaExpression(expression, this);
                }
            });
        }

        public override string ToString()
        {
            return Content.ToString();
        }

        public static NumberExpression Zero => new NumberExpression(NumberDecimal.Zero);
        public static NumberExpression One => new NumberExpression(NumberDecimal.One);
        public static NumberExpression MinusOne => new NumberExpression(NumberDecimal.MinusOne);
    }

    public class OpAddExpression:IExpression
    {
        public OpAddExpression(IExpression left, IExpression right)
        {
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
        }

        public IExpression Right { get; }

        public IExpression Left { get; }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression, (a, b) => new FormulaExpression(a.Left, a.Right, b));
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            return Left.Evaluate(environment).Add(Right.Evaluate(environment));
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            var RightF = Right.Format(environment);
            var LeftF = Left.Format(environment);

            {
                if (RightF is NumberExpression r && LeftF is NumberExpression l)
                {
                    return new NumberExpression(r.GetNumber().Add(l.GetNumber()));
                }
            }
            {
                if (RightF is FormulaExpression r)
                {
                    return r.Add(Left);
                }
            }
            {
                if (LeftF is FormulaExpression l)
                {
                    return l.Add(Right);
                }
            }
            return new FormulaExpression(Right, Left);
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new OpAddExpression(func(this.Left), func(this.Right));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression, () => this.MemberSelect(a => a.Multiply(expression)));
        }

        public override string ToString()
        {
            return Left.ToString() + "+" + Right.ToString();
        }
    }

    public class FormulaExpression : IExpression
    {
        public IValue Value { get; }
        public TermExpression[] Terms { get; }
        public IExpression[] Other { get; }

        public override string ToString()
        {
            var result = new List<string>();
            if (!Value.Equals(NumberDecimal.Zero)) result.Add(Value.ToString());
            result.AddRange(Terms.Select(a => a.ToString()));
            result.AddRange(Other.Select(a => a.ToString()));
            return string.Join("+", result);
        }


        public FormulaExpression(IValue value, TermExpression[] terms, IExpression[] other)
        {
            this.Value = value ?? NumberDecimal.Zero;
            this.Terms = terms ?? new TermExpression[0];
            this.Other = other ?? new IExpression[0];
        }

        public FormulaExpression()
        {
            this.Value = NumberDecimal.Zero;
            this.Terms = new TermExpression[0];
            this.Other = new IExpression[0];
        }

        public FormulaExpression(params IExpression[] expressions)
        {
            var result = new FormulaExpression();
            foreach(var item in expressions)
            {
                result = result.Add(item);
            }
            this.Value = result.Value;
            this.Terms = result.Terms;
            this.Other = result.Other;
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            IValue result = Value;
            foreach(var item in Terms)
            {
                result = result.Add(item.Evaluate(environment));
            }
            foreach (var item in Other)
            {
                result = result.Add(item.Evaluate(environment));
            }
            return result;
        }

        public IExpression Format(Environment.Environment environment) => Format();

        public IExpression Format() => this;

        public FormulaExpression Add(IValue value)
        {
            return new FormulaExpression(this.Value.Add(value), Terms, Other);
        }

        public FormulaExpression Add(NumberExpression value)
        {
            return this.Add(value.Content);
        }

        public FormulaExpression Add(params TermExpression[] terms)
        {
            var termList = this.Terms.ToList();
            foreach(var term in terms){
                for (int i = 0; i < termList.Count(); i++)
                {
                    var temp = TermExpression.AddIfCombinedable(termList[i], term);
                    if (temp != null)
                    {
                        termList[i] = temp;
                        goto BREAKER;
                    }
                }
                termList.Add(term);
                BREAKER:;
            }
            return new FormulaExpression(Value, termList.ToArray(), Other);
        }

        public FormulaExpression Add(FormulaExpression value)
        {
            var result = this;
            result = result.Add(value.Value);
            result = result.Add(value.Terms);
            var other = result.Other.ToList();
            other.AddRange(value.Other);
            result = new FormulaExpression(result.Value, result.Terms, other.ToArray());
            return result;
        }

        public FormulaExpression Add(IExpression expression)
        {
            expression = expression.Format();
            switch (expression)
            {
                case NumberExpression n:return this.Add(n);
                case TermExpression n:return this.Add(n);
                case VariableExpression n:return this.Add(n);
                case FormulaExpression n:return this.Add(n);
                default:
                    var other = this.Other.ToList();
                    other.Add(expression);
                    return new FormulaExpression(this.Value, this.Terms, other.ToArray());
            }
        }

        public FormulaExpression Multiply(IValue value)
        {
            return new FormulaExpression(this.Value.Multiply(value),
                Terms.Select(a=>a.Multiply(value)).ToArray(),
                Other.Select(a=>a.Multiply(new NumberExpression(value))).ToArray());
        }

        public FormulaExpression Multiply(NumberExpression value)
        {
            return this.Multiply(value.Content);
        }

        IExpression IExpression.Add(IExpression expression)
        {
            return Add(expression);
        }

        public IExpression Multiply(IExpression expression)
        {
            { if (expression is NumberExpression number && number.Content == NumberDecimal.Zero) return NumberExpression.Zero; }
            { if (expression is NumberExpression number && number.Content == NumberDecimal.One) return this; }
            if (expression is NumberExpression n) return Multiply(n.Content);
            return MemberSelect(a => a.Multiply(expression));
        }

        public FormulaExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new FormulaExpression(GetMembers().Select(a => func(a)).ToArray());
        }

        public IExpression[] GetMembers()
        {
            List<IExpression> expressions = new List<IExpression>();
            expressions.Add(new NumberExpression(this.Value));
            expressions.AddRange(this.Terms);
            expressions.AddRange(this.Other);
            return expressions.ToArray();
        }

        IExpression IExpression.MemberSelect(Func<IExpression, IExpression> func)
        {
            return MemberSelect(func);
        }
    }

    public class TermExpression : IExpression
    {
        public override string ToString()
        {
            var result = new List<string>();
            if (!Coefficient.Equals(NumberDecimal.One)) result.Add(Coefficient.ToString());
            result.AddRange(Variables.Select(a => a.ToString()));
            return string.Join("", result);
        }

        public TermExpression(IValue coefficient, params VariablePowExpression[] variables)
        {
            Coefficient = coefficient ?? new NumberDecimal(1,0);
            Variables = variables.OrderBy(a=>a.Variable.Variable).ToArray() ?? new VariablePowExpression[0];
        }

        public TermExpression(IValue coefficient, params VariableExpression[] variables)
        {
            Coefficient = coefficient ?? new NumberDecimal(1, 0);
            Variables = variables.OrderBy(a => a.Variable).Select(a => new VariablePowExpression(a)).ToArray() ?? new VariablePowExpression[0];
        }

        public IValue Coefficient { get; }
        public VariablePowExpression[] Variables { get; } = new VariablePowExpression[0];

        public IValue Evaluate(Environment.Environment environment)
        {
            IValue result = Coefficient;
            foreach(var item in Variables)
            {
                result.Multiply(item.Evaluate(environment));
            }
            return result;
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment) => this;

        public static TermExpression AddIfCombinedable(TermExpression a, TermExpression b)
        {
            if (a.Variables.Count() != b.Variables.Count()) return null;
            for(int i = 0; i < a.Variables.Count(); i++)
            {
                if (a.Variables[i].Variable.Variable != b.Variables[i].Variable.Variable) return null;
                if (!a.Variables[i].Exponent.Content.Equals( b.Variables[i].Exponent.Content)) return null;
            }
            return new TermExpression(a.Coefficient.Add(b.Coefficient), a.Variables);
        }


        public TermExpression Multiply(TermExpression value)
        {
            var result = this.Multiply(value.Coefficient);
            foreach (var item in value.Variables)
            {
                result = result.Multiply(item);
            }
            return result;
        }

        public TermExpression Multiply(IValue value)
        {
            return new TermExpression(this.Coefficient.Multiply(value), this.Variables);
        }

        public TermExpression Multiply(params VariablePowExpression[] values)
        {
            var result = this.Variables.ToList();
            foreach (var value in values)
            {
                if (result.Count(a => a.Variable.Variable == value.Variable.Variable) == 0)
                {
                    result.Add(value);
                }
                else
                {
                    result = result
                        .Select(a => a.Variable.Variable != value.Variable.Variable ? a :
                            new VariablePowExpression(a.Variable, new NumberExpression(a.Exponent.Content.Add(value.Exponent.Content)))
                    ).ToList();
                }
            }
            return new TermExpression(this.Coefficient, result.ToArray());
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression, () => new OpAddExpression(this, expression));
        }

        public IExpression Multiply(IExpression ex)
        {
            return Helper.ExpressionMul(this, ex, ()=>{
                switch (ex)
                {
                    case NumberExpression expression: return new TermExpression(this.Coefficient.Multiply(expression.Content), this.Variables);
                    case VariablePowExpression expression: return this.Multiply(expression);
                    case VariableExpression expression: return this.Multiply(new VariablePowExpression(expression));
                    case TermExpression expression:
                        {
                            var result = this.Multiply(expression.Coefficient);
                            return result.Multiply(expression.Variables);
                        }
                    default: return new OpMulExpression(this, ex);
                }
            });
        }


        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return func(this);
        }

        public static implicit operator TermExpression(VariablePowExpression value)
        {
            if (value == null) return null;
            return new TermExpression(NumberDecimal.One, value);
        }

        public static implicit operator TermExpression(VariableExpression value)
        {
            if (value == null) return null;
            return new TermExpression(NumberDecimal.One, value);
        }
    }

    public class OpSubExpression : IExpression
    {
        public override string ToString()
        {
            return Left.ToString() + "-" + Right.ToString();
        }

        public OpSubExpression(IExpression left, IExpression right)
        {
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
        }

        public IExpression Right { get; }

        public IExpression Left { get; }

        public IValue Evaluate(Environment.Environment environment)
        {
            return Left.Evaluate(environment).Substract(Right.Evaluate(environment));
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            return new OpAddExpression(Left, GetRightAsMinus()).Format(environment);
        }

        public IExpression GetRightAsMinus()
        {
            return Right.Multiply(NumberExpression.MinusOne);
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression, () => new FormulaExpression(Left, GetRightAsMinus(), expression));
        }

        public IExpression Multiply(IExpression expression)
        {
            return MemberSelect((a) => a.Multiply(expression));
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new OpSubExpression(func(Left), func(Right));
        }
    }

    public class OpMulExpression : IExpression
    {
        public override string ToString()
        {
            return Left.ToString() + "*" + Right.ToString();
        }

        public OpMulExpression(IExpression left, IExpression right)
        {
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
        }

        public IExpression Right { get; }

        public IExpression Left { get; }

        public IValue Evaluate(Environment.Environment environment)
        {
            return Left.Evaluate(environment).Multiply(Right.Evaluate(environment));
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            var RightF = Right.Format(environment);
            var LeftF = Left.Format(environment);

            var rn = RightF as NumberExpression;
            var ln = LeftF as NumberExpression;

            {
                if (rn != null && ln != null)
                {
                    return new NumberExpression(rn.Content.Multiply(ln.Content));
                }
                if ((rn != null && rn.Content.Equals(NumberDecimal.Zero)) || (ln != null && ln.Content.Equals(NumberDecimal.Zero)))
                {
                    return new NumberExpression(NumberDecimal.Zero);
                }
            }

            TermExpression lt = (LeftF as TermExpression) ?? (LeftF as VariableExpression);
            TermExpression rt = (RightF as TermExpression) ?? (RightF as VariableExpression);

            if (rn != null && lt != null)
            {
                return lt.Multiply(rn.Content);
            }
            if (ln != null && rt != null)
            {
                return rt.Multiply(ln.Content);
            }
            if (rt != null && lt != null)
            {
                return rt.Multiply(lt);
            }
            return new OpMulExpression(LeftF, RightF);
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this,expression,()=> Left.Multiply(Right).Add(expression));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression, () => Left.Multiply(Right).Multiply(expression));
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new OpMulExpression(func(Left), func(Right));
        }

        public static IExpression GetNegate(IExpression expression)
        {
            return NumberExpression.MinusOne.Multiply(expression);
        }
    }

    public class OpPowExpression:IExpression
    {
        public OpPowExpression(IExpression @base, IExpression exponent)
        {
            Base = @base ?? throw new ArgumentNullException(nameof(@base));
            Exponent = exponent ?? throw new ArgumentNullException(nameof(exponent));
        }

        public IExpression Base { get; }
        public IExpression Exponent { get; }

        public IValue Evaluate(Environment.Environment environment)
        {
            throw new NotImplementedException();
        }

        public IExpression Format()
        {
            //ToDo: fix.
            return this;
        }

        public IExpression Format(Environment.Environment environment)
        {
            //ToDo: fix.
            return new OpPowExpression(Base.Format(environment), Exponent.Format(environment));
        }

        public override string ToString()
        {
            return Base.ToString() + "^" + Exponent.ToString();
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new OpPowExpression(func(Base), func(Exponent));
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression, () => new OpAddExpression(this, expression));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression, () => new OpMulExpression(this, expression));
        }
    }

    public class OpFactorialExpression : IExpression
    {
        public OpFactorialExpression(IExpression n)
        {
            N = n ?? throw new ArgumentNullException(nameof(n));
        }

        public IExpression N { get; }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression, () => GetExpression().Add(expression));
        }


        public IExpression GetExpression()
        {
            if (N is NumberExpression n)
            {
                var num = n.Content.GetInt();
                if (num.WithinRange && num.Precise)
                {
                    var f = MathEx.Factorial(num.Value);
                    if (f >= 0) return this;
                }
                return this;
            }
            return this;
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            var a = N.Evaluate(environment).GetInt();
            if (!a.WithinRange) return NumberDecimal.Zero;
            return new NumberDecimal(MathEx.Factorial(a.Value), 0);
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            var n = N.Format(environment);
            if (n is NumberExpression number)
            {
                var a = number.Content.GetInt();
                if (a.WithinRange)
                {
                    return new NumberExpression(new NumberDecimal(MathEx.Factorial(a.Value), 0));
                }
            }
            return new OpFactorialExpression(n);
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new OpFactorialExpression(func(N));
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression, () => GetExpression().Multiply(expression));
        }

        public override string ToString()
        {
            return N.ToString() + "!";
        }
    }

    public class OpDivExpression : IExpression
    {
        public override string ToString()
        {
            return Left.ToString() + "/" + Right.ToString();
        }

        public OpDivExpression(IExpression left, IExpression right)
        {
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
        }

        public IExpression Right { get; }

        public IExpression Left { get; }

        public IValue Evaluate(Environment.Environment environment)
        {
            return Left.Evaluate(environment).Divide(Right.Evaluate(environment));
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            var leftF = Left.Format();
            var rightF = Right.Format();

            {
                if (leftF is NumberExpression leftN && rightF is NumberExpression rightN)
                {
                    return new NumberExpression(leftN.Content.Divide(rightN.Content));
                }
            }
            //Powerと間違えた。
            //{
            //    if (leftF is VariablePowExpression leftN && rightF is NumberExpression rightN)
            //    {
            //        return new VariablePowExpression(leftN.Variable, new NumberExpression(leftN.Exponent.Content.Add(rightN.Content)));
            //    }
            //}
            //{
            //    if (leftF is VariableExpression leftN && rightF is NumberExpression rightN)
            //    {
            //        return new VariablePowExpression(leftN, rightN);
            //    }
            //}

            return new OpDivExpression(Left.Format(), Right.Format());
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression);
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression);
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new OpDivExpression(func(Left), func(Right));
        }
    }

    public class VariablePowExpression : IExpression
    {
        public override string ToString()
        {
            if (Exponent.Content.Equals(NumberDecimal.One)) return Variable.ToString();
            return Variable.ToString() + "^" + Exponent.ToString();
        }

        public VariablePowExpression(VariableExpression variable, NumberExpression exponent = null)
        {
            Variable = variable ?? throw new ArgumentNullException(nameof(variable));
            Exponent = exponent ?? new NumberExpression(NumberDecimal.One);
        }

        public VariableExpression Variable { get; }
        public NumberExpression Exponent { get; }

        public IValue Evaluate(Environment.Environment environment)
        {
            (int Value, bool Precise, bool WithinRange) a;
            if (Exponent.Content is NumberDecimal n)
            {
                a = n.GetInt();
            }
            else if (Exponent.Content is NumberRational m)
            {
                a = m.GetInt();
            }
            else
            {
                throw new NotImplementedException();
            }
            if (a.WithinRange)
            {
                return Variable.Evaluate(environment).Power(a.Value);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            return this;
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd((TermExpression)this, expression);
        }

        public IExpression Multiply(IExpression expression)
        {
            throw new NotImplementedException();
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return func(this);
        }

        public static implicit operator VariablePowExpression(VariableExpression value)
        {
            return new VariablePowExpression(value);
        }
    }

    public class VariableExpression : IExpression
    {
        public override string ToString()
        {
            return Variable?.ToString();
        }

        public string Variable { get; }

        public VariableExpression(string variable)
        {
            this.Variable = variable ?? throw new ArgumentNullException(nameof(variable));
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            var ex = environment.GetVariable(Variable);
            if (ex == null) return new NumberDecimal(0);
            return ex.Evaluate(environment);
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            if (environment == null) return this;
            var ex = environment.GetVariable(Variable);
            if (ex == null) return this;
            return ex;
        }

        public static bool operator ==(VariableExpression a, VariableExpression b)
        {
            return a?.Variable == b?.Variable;
        }

        public static bool operator !=(VariableExpression a, VariableExpression b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Variable.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VariableExpression);
        }

        public IExpression Add(IExpression expression)
        {
            throw new NotImplementedException();
        }

        public IExpression Multiply(IExpression expression)
        {
            throw new NotImplementedException();
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            throw new NotImplementedException();
        }
    }

    public class ArgumentExpression : IExpression
    {
        public override string ToString()
        {
            return String.Join(",", Arguments.Select(a => a.ToString()));
        }

        public IExpression[] Arguments { get; }

        public ArgumentExpression(IExpression left, IExpression right)
        {
            left = left ?? throw new ArgumentNullException(nameof(left));
            right = right ?? throw new ArgumentNullException(nameof(right));
            List<IExpression> result;
            if(left is ArgumentExpression expressionL)
            {
                result = expressionL.Arguments.ToList();
            }
            else
            {
                result = new List<IExpression>() { left };
            }
            if (right is ArgumentExpression expressionR)
            {
                result.AddRange(expressionR.Arguments);
            }
            else
            {
                result.Add(right);
            }
            this.Arguments = result.ToArray();
        }

        public ArgumentExpression(IExpression[] arguments)
        {
            Arguments = arguments ?? new IExpression[0];
        }

        public IValue Evaluate(Environment.Environment environment)
        {
            return Arguments[0].Evaluate(environment);
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            return new ArgumentExpression(this.Arguments.Select(a => a.Format(environment)).ToArray());
        }

        public ArgumentExpression MemberSelect(Func<IExpression,IExpression> converter)
        {
            return new ArgumentExpression(this.Arguments.Select(a => converter(a)).ToArray());
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression);
        }

        public IExpression Multiply(IExpression expression)
        {
            return MemberSelect((a) => a.Multiply(expression));
        }

        IExpression IExpression.MemberSelect(Func<IExpression, IExpression> func)
        {
            return MemberSelect(func);
        }
    }

    public class FuncExpression : IExpression
    {
        public override string ToString()
        {
            return String.Format("{0}({1})", Name, Argument.ToString());
        }

        public FuncExpression(string name,  IExpression arg)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Argument = arg == null ? new ArgumentExpression(new IExpression[0]) :
                (arg is ArgumentExpression argument ? argument : new ArgumentExpression(new IExpression[] { arg }));
        }

        public string Name { get; }
        public ArgumentExpression Argument { get; }

        public IValue Evaluate(Environment.Environment environment)
        {
            var f = environment.GetFunction(Name);
            if(f!=null)
            {
                return f.Evaluate(environment, Argument).Evaluate(environment);
            }
            else
            {
                throw new Exception("Function not exist");
            }
        }

        public IExpression Format() => Format(null);

        public IExpression Format(Environment.Environment environment)
        {
            ArgumentExpression args = this.Argument.Format(environment) as ArgumentExpression;
            var f = environment.GetFunction(Name);
            if (f != null && args != null && f.CanEvaluate(args.Arguments.Length))
            {
                var result = f.Evaluate(environment, args.Arguments)?.Format(environment);
                if (result == null) return new FuncExpression(this.Name, args);
                else return result;
            }
            else
            {
                return new FuncExpression(this.Name, args);
            }
        }

        public IExpression Add(IExpression expression)
        {
            return Helper.ExpressionAdd(this, expression);
        }

        public IExpression Multiply(IExpression expression)
        {
            return Helper.ExpressionMul(this, expression);
        }

        public IExpression MemberSelect(Func<IExpression, IExpression> func)
        {
            return new FuncExpression(this.Name, func(this.Argument));
        }
    }
}
