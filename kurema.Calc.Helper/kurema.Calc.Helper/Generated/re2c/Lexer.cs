/* Generated by re2c 0.16 on Fri Aug  3 04:46:20 2018 */
﻿namespace kurema.Calc.Helper.Interpreter
{
    public class Lexer : yyParser.yyInput
    {
        private string _script;
        private string _value;
        private int    _index;
        private int    _token;
        private int    _cursor;
        private int    _marker;
		private kurema.Calc.Helper.Environment.Environment _environment;

		public Lexer (string script, kurema.Calc.Helper.Environment.Environment environment)
        {
            _script = script;
			_environment = environment;
        }
		
		public bool advance ()
        {
            if ( _script.Length <= _index )
            {
                _token = Token.yyErrorCode;
                _value = "";
                return false;
            }

            _token = Lex(_index);
            _value = currentValue();
            _index = _cursor;

            return true;
        }

		public string currentValue(){
		   return _script.Substring(_index, (_cursor - _index));
		}

        public int token ()
        {
            return _token;
        }

        public object value ()
        {
            switch (_token)
            {
                case Token.NUM: 
                    return _value;

                default:
                    return _value;
            }
        }

        private int YYPEEK ()
        {
            if (_cursor == _script.Length)
            {
                return 0;
            }

            return (int)_script[_cursor];
        }

        private void YYSKIP ()
        {
            _cursor++;
        }

        private void YYBACKUP ()
        {
            _marker = _cursor;
        }

        private void YYRESTORE()
        {
            _cursor = _marker;
        }

        private int Lex (int startIndex)
        {
            _cursor = startIndex;
            YYBACKUP();
loop:
            
{
	int yych;
	yych = YYPEEK ();
	switch (yych) {
	case '\t':
	case '\n':
	case '\v':
	case '\r':
	case ' ':	goto yy3;
	case '!':	goto yy6;
	case '(':	goto yy8;
	case ')':	goto yy10;
	case '*':	goto yy12;
	case '+':	goto yy14;
	case ',':	goto yy16;
	case '-':	goto yy18;
	case '/':	goto yy20;
	case '0':
	case '1':
	case '2':
	case '3':
	case '4':
	case '5':
	case '6':
	case '7':
	case '8':
	case '9':	goto yy22;
	case 'A':
	case 'B':
	case 'C':
	case 'D':
	case 'E':
	case 'F':
	case 'G':
	case 'H':
	case 'I':
	case 'J':
	case 'K':
	case 'L':
	case 'M':
	case 'N':
	case 'O':
	case 'P':
	case 'Q':
	case 'R':
	case 'S':
	case 'T':
	case 'U':
	case 'V':
	case 'W':
	case 'X':
	case 'Y':
	case 'Z':
	case '_':
	case 'a':
	case 'b':
	case 'c':
	case 'd':
	case 'e':
	case 'f':
	case 'g':
	case 'h':
	case 'i':
	case 'j':
	case 'k':
	case 'l':
	case 'm':
	case 'n':
	case 'o':
	case 'p':
	case 'q':
	case 'r':
	case 's':
	case 't':
	case 'u':
	case 'v':
	case 'w':
	case 'x':
	case 'y':
	case 'z':	goto yy25;
	case '^':	goto yy28;
	default:	goto yy2;
	}
yy2:
	YYRESTORE ();
	goto yy24;
yy3:
	YYSKIP ();
	yych = YYPEEK ();
	switch (yych) {
	case '\t':
	case '\n':
	case '\v':
	case '\r':
	case ' ':	goto yy3;
	default:	goto yy5;
	}
yy5:
	{ _index = _cursor; goto loop; }
yy6:
	YYSKIP ();
	{ return Token.OP_FACT;  }
yy8:
	YYSKIP ();
	{ return Token.LP;  }
yy10:
	YYSKIP ();
	{ return Token.RP;  }
yy12:
	YYSKIP ();
	{ return Token.OP_MUL;  }
yy14:
	YYSKIP ();
	{ return Token.OP_ADD;  }
yy16:
	YYSKIP ();
	{ return Token.COMMA;  }
yy18:
	YYSKIP ();
	{ return Token.OP_SUB;  }
yy20:
	YYSKIP ();
	{ return Token.OP_DIV;  }
yy22:
	YYSKIP ();
	YYBACKUP ();
	yych = YYPEEK ();
	switch (yych) {
	case '.':	goto yy30;
	case '0':
	case '1':
	case '2':
	case '3':
	case '4':
	case '5':
	case '6':
	case '7':
	case '8':
	case '9':	goto yy22;
	case 'E':
	case 'e':	goto yy31;
	default:	goto yy24;
	}
yy24:
	{ return Token.NUM;  }
yy25:
	YYSKIP ();
	yych = YYPEEK ();
	switch (yych) {
	case 'A':
	case 'B':
	case 'C':
	case 'D':
	case 'E':
	case 'F':
	case 'G':
	case 'H':
	case 'I':
	case 'J':
	case 'K':
	case 'L':
	case 'M':
	case 'N':
	case 'O':
	case 'P':
	case 'Q':
	case 'R':
	case 'S':
	case 'T':
	case 'U':
	case 'V':
	case 'W':
	case 'X':
	case 'Y':
	case 'Z':
	case '_':
	case 'a':
	case 'b':
	case 'c':
	case 'd':
	case 'e':
	case 'f':
	case 'g':
	case 'h':
	case 'i':
	case 'j':
	case 'k':
	case 'l':
	case 'm':
	case 'n':
	case 'o':
	case 'p':
	case 'q':
	case 'r':
	case 's':
	case 't':
	case 'u':
	case 'v':
	case 'w':
	case 'x':
	case 'y':
	case 'z':	goto yy25;
	default:	goto yy27;
	}
yy27:
	{ 
			     if(_environment.GetFunction(currentValue())!=null) return Token.KEYWORD_FUNC;
			     if(_environment.GetVariable(currentValue())!=null) return Token.KEYWORD_VAR;
				 return Token.KEYWORD_VAR;
			  }
yy28:
	YYSKIP ();
	{ return Token.OP_POW;  }
yy30:
	YYSKIP ();
	yych = YYPEEK ();
	switch (yych) {
	case '0':
	case '1':
	case '2':
	case '3':
	case '4':
	case '5':
	case '6':
	case '7':
	case '8':
	case '9':	goto yy32;
	default:	goto yy2;
	}
yy31:
	YYSKIP ();
	yych = YYPEEK ();
	switch (yych) {
	case '+':
	case '-':	goto yy34;
	case '0':
	case '1':
	case '2':
	case '3':
	case '4':
	case '5':
	case '6':
	case '7':
	case '8':
	case '9':	goto yy35;
	default:	goto yy2;
	}
yy32:
	YYSKIP ();
	YYBACKUP ();
	yych = YYPEEK ();
	switch (yych) {
	case '0':
	case '1':
	case '2':
	case '3':
	case '4':
	case '5':
	case '6':
	case '7':
	case '8':
	case '9':	goto yy32;
	case 'E':
	case 'e':	goto yy31;
	default:	goto yy24;
	}
yy34:
	YYSKIP ();
	yych = YYPEEK ();
	switch (yych) {
	case '0':
	case '1':
	case '2':
	case '3':
	case '4':
	case '5':
	case '6':
	case '7':
	case '8':
	case '9':	goto yy35;
	default:	goto yy2;
	}
yy35:
	YYSKIP ();
	yych = YYPEEK ();
	switch (yych) {
	case '0':
	case '1':
	case '2':
	case '3':
	case '4':
	case '5':
	case '6':
	case '7':
	case '8':
	case '9':	goto yy35;
	default:	goto yy24;
	}
}

        }
    }
}