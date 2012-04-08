using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Core.Basic.AltParser
{
    public class Function : Variable
    {
        private Addition root = null;
        private List<Parameter> parameters = new List<Parameter>();
        private List<Token> tokens = new List<Token>();

        public Function(Addition pRoot, string pName = "undefined", List<Parameter> pParameters = null, List<Token> pTokens = null) : base(pName)
        {
            root = pRoot;
            name = pName;

            if (pParameters != null)
            {
                parameters = pParameters;
            }

            if (pTokens != null)
            {
                tokens = pTokens;
            }
        }

        public double Evaluate()
        {
            return root.Evaluate();
        }

        public Addition Root
        {
            get
            {
                return root;
            }
            set
            {
                root = value;
            }
        }

        public List<Parameter> Parameters
        {
            get
            {
                return parameters;
            }
            set
            {
                parameters = value;
            }
        }

        public List<Token> Tokens
        {
            get
            {
                return tokens;
            }
        }

        public string TokenString()
        {
            if (tokens.Count == 0)
            {
                return "";
            }

            string result = tokens[0].Content;

            for (int i = 1; i < tokens.Count; i++)
            {
                if (tokens[i].Content == "(")
                {
                    if (!tokens[i - 1].IsLiteral())
                    {
                        result += " ";
                    }
                }
                else if (tokens[i - 1].Content == "(")
                {

                }
                else if (tokens[i].Content == ")")
                {

                }
                else if (tokens[i].Content == ",")
                {
                    
                }
                else if (tokens[i].Content == "!")
                {

                }
                else
                {
                    result += " ";
                }

                result += tokens[i].Content;
            }

            return result;
        }

        public override double Value
        {
            get
            {
                return Evaluate();
            }
            set
            {
                
            }
        }

        public void PrintTree()
        {
            Console.WriteLine("Function");
            PrintTree(0, root);
        }

        private void PrintTree(int offset, Operation op)
        {
            string tab = "";

            for (int i = 0; i < offset; i++)
            {
                tab += " ";
            }
            Console.Write(tab + "->");

            Console.WriteLine(op.GetType().Name);

            foreach (Object o in op.Members)
            {
                if (o is Operation)
                {
                    PrintTree(offset + 1, (Operation)o);
                }
                else if (((UnaryExpression)o).Value is MathFunction)
                {
                    MathFunction func = ((UnaryExpression)o).Value as MathFunction;
                    Console.WriteLine(tab + " ->MathFunction(\"" + func.Name + "\")");
                    //Console.WriteLine(tab + " {");

                    foreach (Function f in func.Parameters)
                    {
                        PrintTree(offset + 2, f.Root);
                    }

                    //Console.WriteLine(tab + " }");
                }
                else if (((UnaryExpression)o).Value is Parameter)
                {
                    Console.WriteLine(tab + " Parameter=" + ((Parameter)((UnaryExpression)o).Value).Name);
                }
                else
                {
                    Console.WriteLine(tab + " " + (((UnaryExpression)o).Value.GetType().Name) + "=" + ((UnaryExpression)o).Evaluate());
                }
            }
        }
    }
}
