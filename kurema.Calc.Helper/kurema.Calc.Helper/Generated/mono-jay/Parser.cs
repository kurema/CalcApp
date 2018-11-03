// created by jay 0.7 (c) 1998 Axel.Schreiner@informatik.uni-osnabrueck.de

#line 1 "Parser.jay"

using System;
using kurema.Calc.Helper.Expressions;
using kurema.Calc.Helper.Values;

namespace kurema.Calc.Helper.Interpreter
{
    public class Parser
    {
        private int yacc_verbose_flag = 0;

#line default

  /** error output stream.
      It should be changeable.
    */
  public System.IO.TextWriter ErrorOutput = System.Console.Out;

  /** simplified error message.
      @see <a href="#yyerror(java.lang.String, java.lang.String[])">yyerror</a>
    */
  public void yyerror (string message) {
    yyerror(message, null);
  }
#pragma warning disable 649
  /* An EOF token */
  public int eof_token;
#pragma warning restore 649
  /** (syntax) error message.
      Can be overwritten to control message format.
      @param message text to be displayed.
      @param expected vector of acceptable tokens, if available.
    */
  public void yyerror (string message, string[] expected) {
    if ((yacc_verbose_flag > 0) && (expected != null) && (expected.Length  > 0)) {
      ErrorOutput.Write (message+", expecting");
      for (int n = 0; n < expected.Length; ++ n)
        ErrorOutput.Write (" "+expected[n]);
        ErrorOutput.WriteLine ();
    } else
      ErrorOutput.WriteLine (message);
  }

  /** debugging support, requires the package jay.yydebug.
      Set to null to suppress debugging messages.
    */
//t  internal yydebug.yyDebug debug;

  protected const int yyFinal = 7;
//t // Put this array into a separate class so it is only initialized if debugging is actually used
//t // Use MarshalByRefObject to disable inlining
//t class YYRules : MarshalByRefObject {
//t  public static readonly string [] yyRule = {
//t    "$accept : Sentence",
//t    "Sentence : Formula",
//t    "Arguments : Formula",
//t    "Arguments : Arguments COMMA Formula",
//t    "Formula : Term",
//t    "Formula : OP_ADD Term",
//t    "Formula : OP_SUB Term",
//t    "Formula : Formula OP_ADD Term",
//t    "Formula : Formula OP_SUB Term",
//t    "Term : Power",
//t    "Term : Term Power",
//t    "Term : Term OP_MUL Power",
//t    "Term : Term OP_DIV Power",
//t    "Coefficient : Num",
//t    "Coefficient : Variable",
//t    "Coefficient : Func",
//t    "Power : Coefficient OP_POW Coefficient",
//t    "Power : Coefficient",
//t    "Power : Coefficient OP_FACT",
//t    "Num : NUM",
//t    "Num : LP Formula RP",
//t    "Variable : KEYWORD_VAR",
//t    "Func : KEYWORD_FUNC LP Arguments RP",
//t    "Func : KEYWORD_FUNC NUM",
//t    "Func : KEYWORD_FUNC Variable",
//t    "Func : KEYWORD_FUNC LP RP",
//t  };
//t public static string getRule (int index) {
//t    return yyRule [index];
//t }
//t}
  protected static readonly string [] yyNames = {    
    "end-of-file",null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,"LP","RP","OP_ADD","OP_SUB",
    "OP_MUL","OP_DIV","OP_POW","OP_FACT","NUM","COMMA","KEYWORD",
    "KEYWORD_VAR","KEYWORD_FUNC",
  };

  /** index-checked interface to yyNames[].
      @param token single character or %token value.
      @return token name or [illegal] or [unknown].
    */
//t  public static string yyname (int token) {
//t    if ((token < 0) || (token > yyNames.Length)) return "[illegal]";
//t    string name;
//t    if ((name = yyNames[token]) != null) return name;
//t    return "[unknown]";
//t  }

#pragma warning disable 414
  int yyExpectingState;
#pragma warning restore 414
  /** computes list of expected tokens on error by tracing the tables.
      @param state for which to compute the list.
      @return list of token names.
    */
  protected int [] yyExpectingTokens (int state){
    int token, n, len = 0;
    bool[] ok = new bool[yyNames.Length];
    if ((n = yySindex[state]) != 0)
      for (token = n < 0 ? -n : 0;
           (token < yyNames.Length) && (n+token < yyTable.Length); ++ token)
        if (yyCheck[n+token] == token && !ok[token] && yyNames[token] != null) {
          ++ len;
          ok[token] = true;
        }
    if ((n = yyRindex[state]) != 0)
      for (token = n < 0 ? -n : 0;
           (token < yyNames.Length) && (n+token < yyTable.Length); ++ token)
        if (yyCheck[n+token] == token && !ok[token] && yyNames[token] != null) {
          ++ len;
          ok[token] = true;
        }
    int [] result = new int [len];
    for (n = token = 0; n < len;  ++ token)
      if (ok[token]) result[n++] = token;
    return result;
  }
  protected string[] yyExpecting (int state) {
    int [] tokens = yyExpectingTokens (state);
    string [] result = new string[tokens.Length];
    for (int n = 0; n < tokens.Length;  n++)
      result[n++] = yyNames[tokens [n]];
    return result;
  }

  /** the generated parser, with debugging messages.
      Maintains a state and a value stack, currently with fixed maximum size.
      @param yyLex scanner.
      @param yydebug debug message writer implementing yyDebug, or null.
      @return result of the last reduction, if any.
      @throws yyException on irrecoverable parse error.
    */
  internal Object yyparse (yyParser.yyInput yyLex, Object yyd)
				 {
//t    this.debug = (yydebug.yyDebug)yyd;
    return yyparse(yyLex);
  }

  /** initial size and increment of the state/value stack [default 256].
      This is not final so that it can be overwritten outside of invocations
      of yyparse().
    */
  protected int yyMax;

  /** executed at the beginning of a reduce action.
      Used as $$ = yyDefault($1), prior to the user-specified action, if any.
      Can be overwritten to provide deep copy, etc.
      @param first value for $1, or null.
      @return first.
    */
  protected Object yyDefault (Object first) {
    return first;
  }

	static int[] global_yyStates;
	static object[] global_yyVals;
#pragma warning disable 649
	protected bool use_global_stacks;
#pragma warning restore 649
	object[] yyVals;					// value stack
	object yyVal;						// value stack ptr
	int yyToken;						// current input
	int yyTop;

  /** the generated parser.
      Maintains a state and a value stack, currently with fixed maximum size.
      @param yyLex scanner.
      @return result of the last reduction, if any.
      @throws yyException on irrecoverable parse error.
    */
  internal Object yyparse (yyParser.yyInput yyLex)
  {
    if (yyMax <= 0) yyMax = 256;		// initial size
    int yyState = 0;                   // state stack ptr
    int [] yyStates;               	// state stack 
    yyVal = null;
    yyToken = -1;
    int yyErrorFlag = 0;				// #tks to shift
	if (use_global_stacks && global_yyStates != null) {
		yyVals = global_yyVals;
		yyStates = global_yyStates;
   } else {
		yyVals = new object [yyMax];
		yyStates = new int [yyMax];
		if (use_global_stacks) {
			global_yyVals = yyVals;
			global_yyStates = yyStates;
		}
	}

    /*yyLoop:*/ for (yyTop = 0;; ++ yyTop) {
      if (yyTop >= yyStates.Length) {			// dynamically increase
        global::System.Array.Resize (ref yyStates, yyStates.Length+yyMax);
        global::System.Array.Resize (ref yyVals, yyVals.Length+yyMax);
      }
      yyStates[yyTop] = yyState;
      yyVals[yyTop] = yyVal;
//t      if (debug != null) debug.push(yyState, yyVal);

      /*yyDiscarded:*/ while (true) {	// discarding a token does not change stack
        int yyN;
        if ((yyN = yyDefRed[yyState]) == 0) {	// else [default] reduce (yyN)
          if (yyToken < 0) {
            yyToken = yyLex.advance() ? yyLex.token() : 0;
//t            if (debug != null)
//t              debug.lex(yyState, yyToken, yyname(yyToken), yyLex.value());
          }
          if ((yyN = yySindex[yyState]) != 0 && ((yyN += yyToken) >= 0)
              && (yyN < yyTable.Length) && (yyCheck[yyN] == yyToken)) {
//t            if (debug != null)
//t              debug.shift(yyState, yyTable[yyN], yyErrorFlag-1);
            yyState = yyTable[yyN];		// shift to yyN
            yyVal = yyLex.value();
            yyToken = -1;
            if (yyErrorFlag > 0) -- yyErrorFlag;
            goto continue_yyLoop;
          }
          if ((yyN = yyRindex[yyState]) != 0 && (yyN += yyToken) >= 0
              && yyN < yyTable.Length && yyCheck[yyN] == yyToken)
            yyN = yyTable[yyN];			// reduce (yyN)
          else
            switch (yyErrorFlag) {
  
            case 0:
              yyExpectingState = yyState;
              // yyerror(String.Format ("syntax error, got token `{0}'", yyname (yyToken)), yyExpecting(yyState));
//t              if (debug != null) debug.error("syntax error");
              if (yyToken == 0 /*eof*/ || yyToken == eof_token) throw new yyParser.yyUnexpectedEof ();
              goto case 1;
            case 1: case 2:
              yyErrorFlag = 3;
              do {
                if ((yyN = yySindex[yyStates[yyTop]]) != 0
                    && (yyN += Token.yyErrorCode) >= 0 && yyN < yyTable.Length
                    && yyCheck[yyN] == Token.yyErrorCode) {
//t                  if (debug != null)
//t                    debug.shift(yyStates[yyTop], yyTable[yyN], 3);
                  yyState = yyTable[yyN];
                  yyVal = yyLex.value();
                  goto continue_yyLoop;
                }
//t                if (debug != null) debug.pop(yyStates[yyTop]);
              } while (-- yyTop >= 0);
//t              if (debug != null) debug.reject();
              throw new yyParser.yyException("irrecoverable syntax error");
  
            case 3:
              if (yyToken == 0) {
//t                if (debug != null) debug.reject();
                throw new yyParser.yyException("irrecoverable syntax error at end-of-file");
              }
//t              if (debug != null)
//t                debug.discard(yyState, yyToken, yyname(yyToken),
//t  							yyLex.value());
              yyToken = -1;
              goto continue_yyDiscarded;		// leave stack alone
            }
        }
        int yyV = yyTop + 1-yyLen[yyN];
//t        if (debug != null)
//t          debug.reduce(yyState, yyStates[yyV-1], yyN, YYRules.getRule (yyN), yyLen[yyN]);
        yyVal = yyV > yyTop ? null : yyVals[yyV]; // yyVal = yyDefault(yyV > yyTop ? null : yyVals[yyV]);
        switch (yyN) {
case 2:
#line 55 "Parser.jay"
  {
    yyVal = ((IExpression)yyVals[0+yyTop]);
  }
  break;
case 3:
#line 59 "Parser.jay"
  {
    yyVal = new ArgumentExpression(((IExpression)yyVals[-2+yyTop]), ((IExpression)yyVals[0+yyTop]));
  }
  break;
case 4:
#line 65 "Parser.jay"
  {
    yyVal = ((IExpression)yyVals[0+yyTop]);
  }
  break;
case 5:
#line 69 "Parser.jay"
  {
    yyVal = ((IExpression)yyVals[0+yyTop]);
  }
  break;
case 6:
#line 73 "Parser.jay"
  {
    yyVal = new OpSubExpression( new NumberExpression(new NumberDecimal(0)), ((IExpression)yyVals[0+yyTop]));
  }
  break;
case 7:
#line 77 "Parser.jay"
  {
    yyVal = new OpAddExpression(((IExpression)yyVals[-2+yyTop]), ((IExpression)yyVals[0+yyTop]));
  }
  break;
case 8:
#line 81 "Parser.jay"
  {
    yyVal = new OpSubExpression(((IExpression)yyVals[-2+yyTop]), ((IExpression)yyVals[0+yyTop]));
  }
  break;
case 9:
#line 87 "Parser.jay"
  {
    yyVal = ((IExpression)yyVals[0+yyTop]);
  }
  break;
case 10:
#line 91 "Parser.jay"
  {
    yyVal = new OpMulExpression(((IExpression)yyVals[-1+yyTop]), ((IExpression)yyVals[0+yyTop]));
  }
  break;
case 11:
#line 95 "Parser.jay"
  {
    yyVal = new OpMulExpression(((IExpression)yyVals[-2+yyTop]), ((IExpression)yyVals[0+yyTop]));
  }
  break;
case 12:
#line 99 "Parser.jay"
  {
    yyVal = new OpDivExpression(((IExpression)yyVals[-2+yyTop]), ((IExpression)yyVals[0+yyTop]));
  }
  break;
case 13:
#line 105 "Parser.jay"
  {
    yyVal = ((IExpression)yyVals[0+yyTop]);
  }
  break;
case 14:
#line 109 "Parser.jay"
  {
    yyVal = ((IExpression)yyVals[0+yyTop]);
  }
  break;
case 15:
#line 113 "Parser.jay"
  {
    yyVal = ((IExpression)yyVals[0+yyTop]);
  }
  break;
case 16:
#line 119 "Parser.jay"
  {
    yyVal = new OpPowExpression(((IExpression)yyVals[-2+yyTop]), ((IExpression)yyVals[0+yyTop]));
  }
  break;
case 17:
#line 123 "Parser.jay"
  {
    yyVal = ((IExpression)yyVals[0+yyTop]);
  }
  break;
case 18:
#line 127 "Parser.jay"
  {
    yyVal = new OpFactorialExpression(((IExpression)yyVals[-1+yyTop]));
  }
  break;
case 19:
#line 133 "Parser.jay"
  {
    yyVal = new NumberExpression(new NumberDecimal(((string)yyVals[0+yyTop])));
  }
  break;
case 20:
#line 137 "Parser.jay"
  {
    yyVal = ((IExpression)yyVals[-1+yyTop]);
  }
  break;
case 21:
#line 143 "Parser.jay"
  {
    yyVal = new VariableExpression(((string)yyVals[0+yyTop]));
  }
  break;
case 22:
#line 149 "Parser.jay"
  {
    yyVal = new FuncExpression(((string)yyVals[-3+yyTop]), ((IExpression)yyVals[-1+yyTop]));
  }
  break;
case 23:
#line 153 "Parser.jay"
  {
    yyVal = new FuncExpression(((string)yyVals[-1+yyTop]), new NumberExpression(new NumberDecimal(((string)yyVals[0+yyTop]))));
  }
  break;
case 24:
#line 157 "Parser.jay"
  {
    yyVal = new FuncExpression(((string)yyVals[-1+yyTop]), ((IExpression)yyVals[0+yyTop]));
  }
  break;
case 25:
#line 161 "Parser.jay"
  {
    yyVal = new FuncExpression(((string)yyVals[-2+yyTop]), null);
  }
  break;
#line default
        }
        yyTop -= yyLen[yyN];
        yyState = yyStates[yyTop];
        int yyM = yyLhs[yyN];
        if (yyState == 0 && yyM == 0) {
//t          if (debug != null) debug.shift(0, yyFinal);
          yyState = yyFinal;
          if (yyToken < 0) {
            yyToken = yyLex.advance() ? yyLex.token() : 0;
//t            if (debug != null)
//t               debug.lex(yyState, yyToken,yyname(yyToken), yyLex.value());
          }
          if (yyToken == 0) {
//t            if (debug != null) debug.accept(yyVal);
            return yyVal;
          }
          goto continue_yyLoop;
        }
        if (((yyN = yyGindex[yyM]) != 0) && ((yyN += yyState) >= 0)
            && (yyN < yyTable.Length) && (yyCheck[yyN] == yyState))
          yyState = yyTable[yyN];
        else
          yyState = yyDgoto[yyM];
//t        if (debug != null) debug.shift(yyStates[yyTop], yyState);
	 goto continue_yyLoop;
      continue_yyDiscarded: ;	// implements the named-loop continue: 'continue yyDiscarded'
      }
    continue_yyLoop: ;		// implements the named-loop continue: 'continue yyLoop'
    }
  }

/*
 All more than 3 lines long rules are wrapped into a method
*/
#line default
   static readonly short [] yyLhs  = {              -1,
    0,    1,    1,    2,    2,    2,    2,    2,    3,    3,
    3,    3,    8,    8,    8,    5,    5,    5,    4,    4,
    7,    6,    6,    6,    6,
  };
   static readonly short [] yyLen = {           2,
    1,    1,    3,    1,    2,    2,    3,    3,    1,    2,
    3,    3,    1,    1,    1,    3,    1,    2,    1,    3,
    1,    4,    2,    2,    3,
  };
   static readonly short [] yyDefRed = {            0,
    0,    0,    0,   19,   21,    0,    0,    0,    0,   13,
    9,   15,   14,    0,    0,    0,    0,    0,   23,   24,
    0,    0,    0,    0,   10,    0,   18,   20,   25,    0,
    0,    0,    0,   11,   12,   16,   22,    0,    0,
  };
  protected static readonly short [] yyDgoto  = {             7,
   30,    8,    9,   10,   11,   12,   13,   14,
  };
  protected static readonly short [] yySindex = {         -213,
 -213, -206, -206,    0,    0, -230,    0, -238, -208,    0,
    0,    0,    0, -258, -194, -208, -208, -226,    0,    0,
 -206, -206, -206, -206,    0, -206,    0,    0,    0, -254,
 -238, -208, -208,    0,    0,    0,    0, -213, -238,
  };
  protected static readonly short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,   19,   13,    0,
    0,    0,    0,    1,    0,   16,   25,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 -249,   28,   37,    0,    0,    0,    0,    0, -240,
  };
  protected static readonly short [] yyGindex = {            0,
    0,    2,    8,    0,   -9,    0,   -4,   10,
  };
  protected static readonly short [] yyTable = {            25,
   17,   20,   15,   37,   26,   27,   25,   25,    2,   16,
   17,   38,    4,   34,   35,    5,    2,    3,    1,   31,
   21,   22,   25,   25,    6,    3,   18,    7,   32,   33,
    1,   29,    2,    3,   19,   36,    8,    5,    4,   39,
    0,    5,    6,    1,    0,    2,    3,    0,    1,    0,
    1,    4,   23,   24,    5,    6,    4,    0,    4,    5,
    6,    5,    6,   28,   21,   22,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   17,   17,   17,
   17,   17,   17,    0,    0,   17,   17,    0,   17,   17,
    4,    4,    4,    5,    5,    5,    0,    0,    4,    0,
    0,    5,    6,    6,    6,    7,    7,    7,    0,    0,
    6,    0,    0,    7,    8,    8,    8,    0,    0,    0,
    0,    0,    8,
  };
  protected static readonly short [] yyCheck = {             9,
    0,    6,    1,  258,  263,  264,   16,   17,  258,    2,
    3,  266,    0,   23,   24,    0,  266,  258,    0,   18,
  259,  260,   32,   33,    0,  266,  257,    0,   21,   22,
  257,  258,  259,  260,  265,   26,    0,  268,  265,   38,
   -1,  268,  269,  257,   -1,  259,  260,   -1,  257,   -1,
  257,  265,  261,  262,  268,  269,  265,   -1,  265,  268,
  269,  268,  269,  258,  259,  260,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  257,  258,  259,
  260,  261,  262,   -1,   -1,  265,  266,   -1,  268,  269,
  258,  259,  260,  258,  259,  260,   -1,   -1,  266,   -1,
   -1,  266,  258,  259,  260,  258,  259,  260,   -1,   -1,
  266,   -1,   -1,  266,  258,  259,  260,   -1,   -1,   -1,
   -1,   -1,  266,
  };

#line 163 "Parser.jay"

}
#line default
namespace yydebug {
        using System;
	 internal interface yyDebug {
		 void push (int state, Object value);
		 void lex (int state, int token, string name, Object value);
		 void shift (int from, int to, int errorFlag);
		 void pop (int state);
		 void discard (int state, int token, string name, Object value);
		 void reduce (int from, int to, int rule, string text, int len);
		 void shift (int from, int to);
		 void accept (Object value);
		 void error (string message);
		 void reject ();
	 }
	 
	 class yyDebugSimple : yyDebug {
		 void println (string s){
			 Console.Error.WriteLine (s);
		 }
		 
		 public void push (int state, Object value) {
			 println ("push\tstate "+state+"\tvalue "+value);
		 }
		 
		 public void lex (int state, int token, string name, Object value) {
			 println("lex\tstate "+state+"\treading "+name+"\tvalue "+value);
		 }
		 
		 public void shift (int from, int to, int errorFlag) {
			 switch (errorFlag) {
			 default:				// normally
				 println("shift\tfrom state "+from+" to "+to);
				 break;
			 case 0: case 1: case 2:		// in error recovery
				 println("shift\tfrom state "+from+" to "+to
					     +"\t"+errorFlag+" left to recover");
				 break;
			 case 3:				// normally
				 println("shift\tfrom state "+from+" to "+to+"\ton error");
				 break;
			 }
		 }
		 
		 public void pop (int state) {
			 println("pop\tstate "+state+"\ton error");
		 }
		 
		 public void discard (int state, int token, string name, Object value) {
			 println("discard\tstate "+state+"\ttoken "+name+"\tvalue "+value);
		 }
		 
		 public void reduce (int from, int to, int rule, string text, int len) {
			 println("reduce\tstate "+from+"\tuncover "+to
				     +"\trule ("+rule+") "+text);
		 }
		 
		 public void shift (int from, int to) {
			 println("goto\tfrom state "+from+" to "+to);
		 }
		 
		 public void accept (Object value) {
			 println("accept\tvalue "+value);
		 }
		 
		 public void error (string message) {
			 println("error\t"+message);
		 }
		 
		 public void reject () {
			 println("reject");
		 }
		 
	 }
}
// %token constants
 class Token {
  public const int LP = 257;
  public const int RP = 258;
  public const int OP_ADD = 259;
  public const int OP_SUB = 260;
  public const int OP_MUL = 261;
  public const int OP_DIV = 262;
  public const int OP_POW = 263;
  public const int OP_FACT = 264;
  public const int NUM = 265;
  public const int COMMA = 266;
  public const int KEYWORD = 267;
  public const int KEYWORD_VAR = 268;
  public const int KEYWORD_FUNC = 269;
  public const int yyErrorCode = 256;
 }
 namespace yyParser {
  using System;
  /** thrown for irrecoverable syntax errors and stack overflow.
    */
  internal class yyException : System.Exception {
    public yyException (string message) : base (message) {
    }
  }
  internal class yyUnexpectedEof : yyException {
    public yyUnexpectedEof (string message) : base (message) {
    }
    public yyUnexpectedEof () : base ("") {
    }
  }

  /** must be implemented by a scanner object to supply input to the parser.
    */
  internal interface yyInput {
    /** move on to next token.
        @return false if positioned beyond tokens.
        @throws IOException on input error.
      */
    bool advance (); // throws java.io.IOException;
    /** classifies current token.
        Should not be called if advance() returned false.
        @return current %token or single character.
      */
    int token ();
    /** associated with current token.
        Should not be called if advance() returned false.
        @return value for token().
      */
    Object value ();
  }
 }
} // close outermost namespace, that MUST HAVE BEEN opened in the prolog
