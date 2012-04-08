using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Core.Basic.AltParser
{
    public class Tokenizer
    {
        public Tokenizer()
        {

        }

        public List<Token> Tokenize(string input)
        {
            char[] chr = input.ToCharArray();
            List<Token> result = new List<Token>();
            string temp = "";

            for (int i = 0; i < chr.Length; i++)
            {
                if (IsWhitespace(chr[i]))
                {
                    continue;
                }

                if (IsNumber(chr[i]))
                {
                    for (; i < chr.Length && IsNumber(chr[i]); i++)
                    {
                        temp += chr[i];
                    }

                    if (i >= chr.Length)
                    {
                        result.Add(new Token(temp));
                        break;
                    }

                    if (chr[i] == '.')
                    {
                        temp += chr[i];
                        i++;
                    }
                    else
                    {
                        result.Add(new Token(temp));
                        temp = "";
                        i--;
                        continue;
                    }

                    for (; i < chr.Length && IsNumber(chr[i]); i++)
                    {
                        temp += chr[i];
                    }

                    result.Add(new Token(temp));
                    temp = "";

                    i--;
                    continue;
                }
                else if (IsLiteral(chr[i]))
                {
                    for (; i < chr.Length && (IsLiteral(chr[i]) || IsNumber(chr[i])); i++)
                    {
                        temp += chr[i];
                    }

                    result.Add(new Token(temp));
                    temp = "";
                    i--;
                    continue;
                }
                else if (IsToken(chr[i]))
                {
                    char c = chr[i];

                    if (c == '+' || c == '-')
                    {
                        bool neg = (c == '-');
                        i++;

                        for (; i < chr.Length; i++)
                        {
                            if (!IsWhitespace(chr[i]) && chr[i] != '+' && chr[i] != '-')
                            {
                                i--;
                                break;
                            }

                            if (chr[i] == '-')
                            {
                                neg = !neg;
                            }
                        }

                        c = (neg == true) ? '-' : '+';
                    }

                    result.Add(new Token(c));
                    continue;
                }
                else
                {
                    throw new Exception("Illegal character: '" + chr[i] + "' near \"" + input.Substring(0, i) + "\"");
                }
            }

            return result;
        }

        private bool IsNumber(char c)
        {
            return (c >= '0' && c <= '9');
        }

        private bool IsLiteral(char c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_');
        }

        private bool IsToken(char c)
        {
            return (c == '+' ||
                    c == '-' ||
                    c == '*' ||
                    c == '/' ||
                    c == '!' ||
                    c == '(' ||
                    c == ')' ||
                    c == '^' ||
                    c == ':' ||
                    c == '[' ||
                    c == ']' ||
                    c == '=' ||
                    c == '%' ||
                    c == ',');
        }

        private bool IsWhitespace(char c)
        {
            return (c == ' ' ||
                    c == '\t' ||
                    c == '\n' ||
                    c == '\r');
        }
    }
}
