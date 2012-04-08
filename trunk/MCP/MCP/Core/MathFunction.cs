using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCP.Core.Basic.AltParser;

namespace MCP.Core
{
    class MathFunction : Variable
    {
        private List<Function> parameters = null;

        public MathFunction(string name, List<Function> pParameters = null) : base(name)
        {
            parameters = pParameters;

            if (parameters == null)
            {
                parameters = new List<Function>();
            }
        }

        public List<Function> Parameters
        {
            get
            {
                return parameters;
            }
        }

        public override double Value
        {
            get
            {
                return MathPool.Execute(this);
            }
            set
            {
                
            }
        }
    }
}
