using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Core.Basic.AltParser
{
    public class Variable : Element
    {
        protected string name = "undefined";

        public Variable(string pName, double pValue = 0.0) : base(pValue)
        {
            name = pName;
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        private Variable Find()
        {
            Variable var = DataPool.GetInstance().Get(name);

            if (var is Function)
            {
                throw new Exception("\"" + name + "\" is a function and cannot be used as a variable");
            }

            if (!(var is Variable))
            {
                throw new Exception("\"" + name + "\" is not a variable");
            }

            return var;
        }

        public override double Value
        {
            get
            {
                return Find().value;
            }
            set
            {
                Find().value = value;
            }
        }
    }
}
