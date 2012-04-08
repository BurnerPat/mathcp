using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Core.Basic.AltParser
{
    public abstract class Operation
    {
        protected List<object> members = new List<object>();
        protected List<char> operators = new List<char>();

        public Operation(List<object> pMembers = null, List<char> pOperators = null)
        {
            if (pMembers != null)
            {
                members = pMembers;
            }

            if (pOperators != null)
            {
                operators = pOperators;
            }
        }

        public abstract double Evaluate();

        protected double GetValue(object obj)
        {
            if (obj is Operation)
            {
                return ((Operation)obj).Evaluate();
            }
            else if (obj is UnaryExpression)
            {
                return ((UnaryExpression)obj).Evaluate();
            }
            else
            {
                throw new Exception("Invalid object " + obj.GetType().FullName);
            }
        }

        public List<object> Members
        {
            get
            {
                return members;
            }
        }

        public List<char> Operators
        {
            get
            {
                return operators;
            }
        }
    }
}
