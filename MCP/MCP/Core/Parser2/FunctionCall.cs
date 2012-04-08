using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Core.Basic.AltParser
{
    public class FunctionCall : Variable
    {
        private Function func = null;
        private List<Function> parameters = new List<Function>();

        public FunctionCall(Function pFunc) : base("undefined")
        {
            func = pFunc;
        }

        public List<Function> Parameters
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

        public Function Function
        {
            get
            {
                return func;
            }
            set
            {
                func = value;
            }
        }

        public override double Value
        {
            get
            {
                try
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        func.Parameters[i].Value = parameters[i].Value;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new Exception("Invalid number of parameters for function call \"" + name + "\"");
                }

                return func.Evaluate();
            }
            set
            {
                
            }
        }

        public double Evaluate()
        {
            return Value;
        }
    }
}
