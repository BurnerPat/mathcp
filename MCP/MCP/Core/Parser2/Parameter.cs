using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Core.Basic.AltParser
{
    public class Parameter : Element, IEquatable<Parameter>
    {
        protected string name = "undefined";

        public Parameter(string pName, double pValue = 0.0) : base(pValue)
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

        public override double Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.Value = value;
            }
        }

        public bool Equals(Parameter other)
        {
            return (other.Name == name);
        }
    }
}
