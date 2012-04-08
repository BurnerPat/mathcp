using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCP.Core.Basic.AltParser;

namespace MCP.Core
{
    public class MCParser
    {
        private Parser parser = new Parser();
        private Tokenizer tokenizer = new Tokenizer();
        private DataPool data = DataPool.GetInstance();
        private bool debug = false;
        private bool printFunc = false;

        public MCParser()
        {
            
        }

        public string Execute(string cmd)
        {
            List<Token> tokens = new List<Token>();

            try
            {
                tokens = tokenizer.Tokenize(cmd);
            }
            catch (Exception ex)
            {
                return (ex.Message);
            }

            string sys = HandleSystemCommand(tokens);

            if (sys != "")
            {
                return sys;
            }
            else
            {
                try
                {
                    Function func = parser.Parse(tokens);
                    if (debug)
                    {
                        Console.WriteLine("-----------------------");
                        func.PrintTree();
                        Console.WriteLine("-----------------------");
                    }
                    double result = func.Evaluate();
                    data.Ans(result);
                    return "Result: " + ((printFunc) ? (func.TokenString() + " = ") : "") + result;
                }
                catch (Exception ex)
                {
                    return ex.Message + "\nError";
                }
            }
        }

        private string HandleSystemCommand(List<Token> tokens)
        {
            if (tokens.Count == 0)
            {
                return "No input";
            }

            if (!tokens[0].IsLiteral())
            {
                return "";
            }

            switch (tokens[0].Content.ToUpper())
            {
                case "DATA":
                case "TABLE":
                    string str = data.GenerateTable();
                    if (str == null)
                    {
                        return "Variable memory is empty";
                    }
                    else
                    {
                        return "\n" + str;
                    }

                case "FUNCTIONS":
                    string fstr = data.GenerateFunctionTable();
                    if (fstr == null)
                    {
                        return "Function memory is empty";
                    }
                    else
                    {
                        return "\n" + fstr;
                    }

                case "FUNC":
                    printFunc = !printFunc;
                    return (!printFunc) ? "Function output disabled" : "Function output enabled";

                case "FLUSH":
                    data.Clear();
                    return "Data has successfully been reset";

                case "CLS":
                case "CLEAR":
                    Console.Clear();
                    return "Console cleared";

                case "HELP":
                    return HelpMessage();

                case "DEBUG":
                    if (!debug)
                    {
                        debug = true;
                        return "Debug mode enabled";
                    }
                    else
                    {
                        debug = false;
                        return "Debug mode disabled";
                    }
            }

            if (tokens.Count == 1)
            {
                if (tokens[0].IsLiteral())
                {
                    try
                    {
                        Variable var = data.Get(tokens[0].Content);

                        if (var is Function)
                        {
                            return "\"" + var.Name + "\" is a function and cannot be used as a variable";
                        }

                        return var.Name + " = " + var.Value;
                    }
                    catch
                    {
                        return "\"" + tokens[0].Content + "\" could not be resolved";
                    }
                }
            }

            if (tokens.Count > 1)
            {
                if (tokens[1].Content == "=")
                {
                    string result = "";

                    try
                    {
                        List<Token> subList = tokens.GetRange(2, tokens.Count - 2);
                        Function func = parser.Parse(subList);

                        if (!data.Contains(tokens[0].Content))
                        {
                            Variable var = new Variable(tokens[0].Content, func.Evaluate());
                            data.Register(var);
                            result = var.Name + ((printFunc) ? (" = " + func.TokenString()) : "") + " = " + var.Value;
                            data.Ans(var.Value);
                            if (debug)
                            {
                                Console.WriteLine("-----------------------");
                                func.PrintTree();
                                Console.WriteLine("-----------------------");
                            }
                        }
                        else
                        {
                            Variable var = data.Get(tokens[0].Content);
                            Variable nVar = new Variable(var.Name, func.Evaluate());
                            data.Set(var.Name, nVar);
                            result = var.Name + ((printFunc) ? (" = " + func.TokenString()) : "") + " = " + var.Value;
                            data.Ans(var.Value);
                            if (debug)
                            {
                                Console.WriteLine("-----------------------");
                                func.PrintTree();
                                Console.WriteLine("-----------------------");
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return "Syntax error";
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return "Syntax error";
                    }
                    catch (Exception ex)
                    {
                        return ex.GetType().Name + ": " + ex.Message;
                    }

                    return "OK. " + result;
                }
                else if (tokens[1].Content == "(")
                {
                    int o = 2;
                    for (; o < tokens.Count; o++)
                    {
                        if (tokens[o].Content == "=")
                        {
                            break;
                        }
                    }

                    if (o >= tokens.Count)
                    {
                        return "";
                    }

                    string name = tokens[0].Content;
                    List<string> parameters = new List<string>();

                    if (tokens[2].Content != ")")
                    {
                        parameters.Add(tokens[2].Content);
                    }

                    for (int i = 3; i < (o - 2); i += 2)
                    {
                        if (tokens[i].Content != ",")
                        {
                            return "Syntax error: ',' expected";
                        }

                        parameters.Add(tokens[i + 1].Content);
                    }

                    foreach (string p in parameters)
                    {
                        if (data.Contains(p))
                        {
                            return "Error: Parameter \"" + p + "\" is already defined";
                        }
                    }

                    Function f = null;
                    try
                    {
                        f = parser.Parse(tokens.GetRange(o + 1, tokens.Count - (o + 1)));
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }

                    f.Name = name;

                    if (debug)
                    {
                        Console.WriteLine("-----------------------");
                        f.PrintTree();
                        Console.WriteLine("-----------------------");
                    }

                    List<string> unmatched = new List<string>();

                    foreach (Parameter p in f.Parameters)
                    {
                        if (!parameters.Contains(p.Name))
                        {
                            unmatched.Add(p.Name);
                        }
                    }

                    if (unmatched.Count > 0)
                    {
                        string result = "Error: Undefined variable(s) \"" + unmatched[0] + "\"";

                        for (int i = 1; i < unmatched.Count; i++)
                        {
                            result += ", \"" + unmatched[i] + "\"";
                        }

                        return result;
                    }

                    unmatched.Clear();

                    foreach (string s in parameters)
                    {
                        if (!f.Parameters.Exists(x => x.Name == s))
                        {
                            unmatched.Add(s);
                            f.Parameters.Add(new Parameter(s));
                        }
                    }

                    if (unmatched.Count > 0)
                    {
                        string temp = "Warning: Unused parameter(s) \"" + unmatched[0] + "\"";

                        for (int i = 1; i < unmatched.Count; i++)
                        {
                            temp += ", \"" + unmatched[i] + "\"";
                        }

                        Console.WriteLine(temp);
                    }

                    if (data.Contains(f.Name))
                    {
                        Variable v = data.Get(f.Name);

                        if (!(v is Function))
                        {
                            return "Error: Cannot apply function to a variable";
                        }

                        Function func = (Function)v;

                        data.Set(func.Name, f);
                        string result = "OK, " + f.Name + "(";

                        if (parameters.Count > 0)
                        {
                            result += parameters[0];
                        }

                        for (int i = 1; i < parameters.Count; i++)
                        {
                            result += ", " + parameters[i];
                        }

                        result += ") = " + f.TokenString();
                        return result;
                    }
                    else
                    {
                        data.Register(f);
                        string result = "OK, " + f.Name + "(";

                        if (parameters.Count > 0)
                        {
                            result += parameters[0];
                        }

                        for (int i = 1; i < parameters.Count; i++)
                        {
                            result += ", " + parameters[i];
                        }

                        result += ") = " + f.TokenString();
                        return result;
                    }
                }
            }

            return "";
        }

        public string HelpMessage()
        {
            return "Available commands:\n" +
                   "\tDATA / TABLE\t-> Display variable data in memory\n" +
                   "\tFUNCTIONS\t-> Display function data in memory\n" + 
                   "\tFLUSH\t\t-> Reset data memory\n" +
                   "\tDEBUG\t\t-> Enable / disable debug mode [default = OFF]\n" +
                   "\tFUNC\t\t-> Enable / disable function output [default = OFF]\n" +
                   "\tCLS / CLEAR\t-> Clear console\n" +
                   "\tHELP\t\t-> Display this message, doh!\n" +
                   "\tEXIT\t\t-> Exit the program\n\n" + 
                   "All commands are case-insensitive";
        }
    }
}
