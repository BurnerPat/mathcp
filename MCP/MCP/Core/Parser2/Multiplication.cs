using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Core.Basic.AltParser
{
    public class Multiplication : Operation
    {
        public Multiplication(List<object> pMembers = null, List<char> pOperators = null) : base(pMembers, pOperators)
        {

        }

        public override double Evaluate()
        {
            if (members.Count == 0)
            {
                throw new Exception("Empty multiplication");
            }

            double result = GetValue(members[0]);

            try
            {
                for (int i = 0; i < operators.Count; i++)
                {
                    switch (operators[i])
                    {
                        case '*':
                            result *= GetValue(members[i + 1]);
                            break;

                        case '/':
                            double temp = GetValue(members[i + 1]);
                            if (temp != 0.0)
                            {
                                result /= temp;
                            }
                            else
                            {
                                throw new Exception("Division by zero");
                            }
                            break;

                        default:
                            throw new Exception("Invalid operator '" + operators[i] + "' in multiplication");
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Syntax error");
            }

            return result;
        }

        private char Peek(int o)
        {
            try
            {
                return operators[o + 1];
            }
            catch (IndexOutOfRangeException)
            {
                return '0';
            }
        }
    }
}
