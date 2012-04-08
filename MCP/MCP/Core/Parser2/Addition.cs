using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Core.Basic.AltParser
{
    public class Addition : Operation
    {
        public Addition(List<object> pMembers = null, List<char> pOperators = null) : base(pMembers, pOperators)
        {

        }

        public override double Evaluate()
        {
            if (members.Count == 0)
            {
                throw new Exception("Empty addition");
            }

            double result = GetValue(members[0]);

            try
            {
                for (int i = 0; i < operators.Count; i++)
                {
                    switch (operators[i])
                    {
                        case '+':
                            result += GetValue(members[i + 1]);
                            break;

                        case '-':
                            result -= GetValue(members[i + 1]);
                            break;

                        default:
                            throw new Exception("Invalid operator '" + operators[i] + "' in addition");
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Syntax error");
            }

            return result;
        }
    }
}
