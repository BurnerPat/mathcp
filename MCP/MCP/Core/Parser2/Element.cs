using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Core.Basic.AltParser
{
    public class Element
    {
        protected double value = 0.0;

        public Element(double pValue)
        {
            value = pValue;
        }

        public virtual double Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }
    }
}
