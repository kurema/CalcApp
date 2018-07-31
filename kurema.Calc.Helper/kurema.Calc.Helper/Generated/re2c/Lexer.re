namespace kurema.Calc.Helper.Interpreter
{
    public class Lexer : yyParser.yyInput
    {
        private string _script;
        private string _value;
        private int    _index;
        private int    _token;
        private int    _cursor;
        private int    _marker; public Lexer (string script)
        {
            _script = script;
        } public bool advance ()
        {
            if ( _script.Length <= _index )
            {
                _token = Token.yyErrorCode;
                _value = "";
                return false;
            }

            _token = Lex(_index);
            _value = _script.Substring(_index, (_cursor - _index));
            _index = _cursor;

            return true;
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
            /*!re2c
              re2c:define:YYCTYPE = int;
              re2c:yyfill:enable = 0;

              WSP         = [ \t\v\n\r];
              WHITE_SPACE = WSP+;

              OP_ADD      = "+";
              OP_SUB      = "-";
              OP_MUL      = "*";
              OP_DIV      = "/";
              LP          = "(";
              RP          = ")";

              DIGIT       = [0-9];
              DIGITNZ     = [1-9];
              U_INTEGER   = "0" | DIGITNZ DIGIT*;
              DOUBLE      = U_INTEGER "." DIGIT+;
              NUM         = U_INTEGER | DOUBLE;

              WHITE_SPACE   { _index = _cursor; goto loop; }
              OP_ADD        { return Token.OP_ADD;  }
              OP_SUB        { return Token.OP_SUB;  }
              OP_MUL        { return Token.OP_MUL;  }
              OP_DIV        { return Token.OP_DIV;  }
              LP            { return Token.LP;  }
              RP            { return Token.RP;  }
              NUM           { return Token.NUM;  }
            */
        }
    }
}