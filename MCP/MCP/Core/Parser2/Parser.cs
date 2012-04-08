using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Core.Basic.AltParser
{
    public class Parser
    {
        public Parser()
        {

        }

        public Function Parse(List<Token> tokens)
        {
            List<Parameter> dependencies = new List<Parameter>();

            int endoffset = 0;

            try
            {
                Addition root = ParseA(tokens, 0, out endoffset, dependencies);
                
                Function function = new Function(root, "undefined", dependencies, tokens);
                return function;
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Unexpected end of input after \"" + tokens[tokens.Count - 1] + "\"");
            }
        }

        private Addition ParseA(List<Token> tokens, int offset, out int endoffset, List<Parameter> dependencies, bool breakComma = false)
        {
            Addition result = new Addition();

            result.Members.Add(ParseM(tokens, offset, out offset, dependencies, breakComma));

            bool run = true;
            while (offset < tokens.Count && run)
            {
                if (!tokens[offset].IsChar())
                {
                    throw new Exception("Invalid token '" + tokens[offset].Content + "'(" + (offset + 1) + ") in addition");
                }

                switch (tokens[offset].Char)
                {
                    case '+':
                    case '-':
                        result.Operators.Add(tokens[offset].Char);
                        result.Members.Add(ParseM(tokens, offset + 1, out offset, dependencies, breakComma));
                        break;

                    case ')':
                        run = false;
                        break;

                    case ',':
                        if (breakComma)
                        {
                            run = false;
                            break;
                        }
                        else
                        {
                            throw new Exception("Invalid token ','(" + (offset + 1) + ") in addition");
                        }

                    default:
                        throw new Exception("Invalid operator '" + tokens[offset].Char + "'(" + (offset + 1) + ") in addition");
                }
            }

            endoffset = offset;
            return result;
        }

        private Multiplication ParseM(List<Token> tokens, int offset, out int endoffset, List<Parameter> dependencies, bool quitOnBrace = false)
        {
            Multiplication result = new Multiplication();

            if (tokens[offset].IsChar() && tokens[offset].Char == '(')
            {
                result.Members.Add(ParseA(tokens, offset + 1, out offset, dependencies));
            }
            else
            {
                result.Members.Add(ParseT(tokens, offset, out offset, dependencies, quitOnBrace));
            }

            bool run = true;
            while (offset < tokens.Count && run)
            {
                if (!tokens[offset].IsChar())
                {
                    throw new Exception("Invalid token '" + tokens[offset].Content + "'(" + (offset + 1) + ") in multiplication");
                }

                switch (tokens[offset].Char)
                {
                    case '*':
                    case '/':
                        result.Operators.Add(tokens[offset].Char);
                        result.Members.Add(ParseT(tokens, offset + 1, out offset, dependencies, quitOnBrace));
                        break;

                    case '+':
                    case '-':
                    case ',':
                        run = false;
                        break;

                    case ')':
                        run = false;
                        if (!quitOnBrace)
                        {
                            offset++;
                        }
                        break;

                    default:
                        throw new Exception("Invalid operator '" + tokens[offset].Char + "'(" + (offset + 1) + ") in multiplication");
                }
            }

            endoffset = offset;
            return result;
        }

        private TopLevel ParseT(List<Token> tokens, int offset, out int endoffset, List<Parameter> dependencies, bool quitOnBrace = false)
        {
            TopLevel result = new TopLevel();

            if (tokens[offset].IsChar() && tokens[offset].Char == '(')
            {
                result.Members.Add(ParseA(tokens, offset + 1, out offset, dependencies));
            }
            else
            {
                result.Members.Add(PreParseU(tokens, offset, out offset, dependencies));
            }

            bool run = true;
            while (offset < tokens.Count && run)
            {
                if (!tokens[offset].IsChar())
                {
                    throw new Exception("Invalid token '" + tokens[offset].Content + "'(" + (offset + 1) + ") in top-level operation");
                }

                switch (tokens[offset].Char)
                {
                    case '^':
                    case '%':
                        result.Operators.Add(tokens[offset].Char);
                        result.Members.Add(PreParseU(tokens, offset + 1, out offset, dependencies));
                        break;

                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case ',':
                        run = false;
                        break;

                    case ')':
                        run = false;
                        if (!quitOnBrace)
                        {
                            offset++;
                        }
                        break;

                    default:
                        throw new Exception("Invalid operator '" + tokens[offset].Char + "'(" + (offset + 1) + ") in top-level operation");
                }
            }

            endoffset = offset;
            return result;
        }

        private Object PreParseU(List<Token> tokens, int offset, out int endoffset, List<Parameter> dependencies)
        {
            Object result = null;

            if (tokens[offset].IsChar())
            {
                if (tokens[offset].Char == '(')
                {
                    result = ParseA(tokens, offset + 1, out offset, dependencies);
                }
            }

            if (result == null)
            {
                result = ParseU(tokens, offset, out offset, dependencies);
            }
            endoffset = offset;
            return result;
        }

        private UnaryExpression ParseU(List<Token> tokens, int offset, out int endoffset, List<Parameter> dependencies)
        {
            //Console.Write("U");

            char op = '+';
            Element value = null;
            UnaryExpression result = null;

            if (tokens[offset].IsChar() && !tokens[offset].IsLiteral())
            {
                char c = tokens[offset].Char;

                if (c < '0' || c > '9')
                {
                    switch (c)
                    {
                        case '+':
                        case '-':
                            op = c;
                            break;

                        default:
                            throw new Exception("Invalid operator for unary expression: \"" + tokens[offset].Content + "\"");
                    }

                    offset++;
                }
            }

            if (tokens[offset].IsNumeric())
            {
                value = new Element(tokens[offset].ToDouble());
            }
            else
            {
                if (!MathPool.IsMathFunction(tokens[offset].Content))
                {
                    try
                    {
                        value = DataPool.GetInstance().Get(tokens[offset].Content);
                    }
                    catch
                    {
                        if (!dependencies.Exists(x => x.Name == tokens[offset].Content))
                        {
                            value = new Parameter(tokens[offset].Content);
                            dependencies.Add((Parameter)value);
                        }
                        else
                        {
                            value = dependencies[dependencies.FindIndex(x => x.Name == tokens[offset].Content)];
                        }
                    }

                    if (value is Function)
                    {
                        try
                        {
                            value = ParseC(tokens, offset, out offset, dependencies);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            throw new Exception("Syntax error");
                        }
                    }
                }
                else
                {
                    try
                    {
                        value = ParseC(tokens, offset, out offset, dependencies);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        throw new Exception("Syntax error");
                    }
                }
            }
            offset++;

            if (offset < tokens.Count)
            {
                if (tokens[offset].IsChar())
                {
                    if (tokens[offset].Char == '!')
                    {
                        result = new UnaryExpression('!', new UnaryExpression(op, value));
                        offset++;
                    }
                    else
                    {
                        result = new UnaryExpression(op, value);
                    }
                }
                else
                {
                    result = new UnaryExpression(op, value);
                }
            }
            else
            {
                result = new UnaryExpression(op, value);
            }

            endoffset = offset;
            return result;
        }

        private Variable ParseC(List<Token> tokens, int offset, out int endoffset, List<Parameter> dependencies)
        {
            MathFunction result = new MathFunction(tokens[offset].Content);

            if (tokens[offset + 2].IsChar() && tokens[offset + 2].Char == ')')
            {
                if (DataPool.GetInstance().Contains(result.Name))
                {
                    if (!(DataPool.GetInstance().Get(result.Name) is Function))
                    {
                        throw new Exception("Unable to apply function call on variable \"" + result.Name + "\"");
                    }

                    Function f = (Function)DataPool.GetInstance().Get(result.Name);
                    FunctionCall call = new FunctionCall(f);

                    if (f.Parameters.Count > 0)
                    {
                        throw new Exception("\"" + f.Name + "\" expects " + f.Parameters.Count + " parameter(s), but 0 were given");
                    }

                    endoffset = offset + 2;
                    return call;
                }

                endoffset = offset + 2;
                return result;
            }
            offset += 1;

            for (int i = 1; ; i++)
            {
                try
                {
                    result.Parameters.Add(new Function(ParseA(tokens, offset + 1, out offset, dependencies, true)));
                }
                catch (Exception ex)
                {
                    throw new Exception("Error in parameter " + i + " for function \"" + result.Name + "\"\n" + ex.Message);
                }

                if (tokens[offset].IsChar())
                {
                    try
                    {
                        if (tokens[offset].Char == ')')
                        {
                            break;
                        }
                        else if (tokens[offset].Char == ',')
                        {
                            //offset++;
                            continue;
                        }
                        else
                        {
                            throw new Exception("Invalid token '" + tokens[offset].Content + "' in function call for \"" + result.Name + "\"");
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        throw new Exception("Syntax error in parameter " + i + " for function \"" + result.Name + "\"");
                    }
                }
                else
                {
                    throw new Exception("Invalid token '" + tokens[offset].Content + "' in function call for \"" + result.Name + "\"");
                }
            }

            if (DataPool.GetInstance().Contains(result.Name))
            {
                if (!(DataPool.GetInstance().Get(result.Name) is Function))
                {
                    throw new Exception("Unable to apply function call on variable \"" + result.Name + "\"");
                }

                Function f = (Function)DataPool.GetInstance().Get(result.Name);
                FunctionCall call = new FunctionCall(f);

                if (f.Parameters.Count != result.Parameters.Count)
                {
                    throw new Exception("\"" + f.Name + "\" expects " + f.Parameters.Count + " parameter(s), but " + result.Parameters.Count + " were given");
                }

                for (int i = 0; i < result.Parameters.Count; i++)
                {
                    call.Parameters.Add(result.Parameters[i]);
                }

                endoffset = offset;
                return call;
            }

            if (!MathPool.IsMathFunction(result.Name))
            {
                throw new Exception("\"" + result.Name + "\" cannot be resolved");
            }

            endoffset = offset;
            return result;
        }
    }
}
