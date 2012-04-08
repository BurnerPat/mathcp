using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace MCP.Core.Basic.AltParser
{
    public class Token
    {
        private string token = "";

        public Token(string str)
        {
            token = str;
        }

        public Token(char c)
        {
            token = c.ToString();
        }

        public Token()
        {

        }

        public string Content
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
            }
        }

        public bool IsChar()
        {
            return (token.Length == 1);
        }

        public char Char
        {
            get
            {
                if (token.Length == 1)
                {
                    return token[0];
                }
                else
                {
                    throw new Exception("Token \"" + token + "\" was not a character");
                }
            }
            set
            {
                token = value.ToString();
            }
        }

        public bool IsLiteral()
        {
            foreach (char c in token)
            {
                if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && c != '_')
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsNumeric()
        {
            bool dot = false;

            foreach (char c in token)
            {
                if ((c < '0' || c > '9') && c != '.')
                {
                    return false;
                }

                if (c == '.')
                {
                    if (dot)
                    {
                        return false;
                    }
                    else
                    {
                        dot = true;
                    }
                }
            }

            return true;
        }

        public double ToDouble()
        {
            return double.Parse(token);
        }

        public override string ToString()
        {
            return token;
        }
    }
}
