using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Core.Basic.AltParser
{
    public class UnaryExpression
    {
        private char op = '+';
        private Object value = null;

        public UnaryExpression(char pOp, Object pValue)
        {
            if (!(pValue is UnaryExpression || pValue is Element))
            {
                throw new Exception("Invalid value set for unary expression: " + pValue.GetType().FullName);
            }

            value = pValue;
            op = pOp;
        }

        public char Operator
        {
            get
            {
                return op;
            }
            set
            {
                op = value;
            }
        }

        public Object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (!(value is UnaryExpression || value is Element))
                {
                    throw new Exception("Invalid value set for unary expression: " + value.GetType().FullName);
                }

                this.value = value;
            }
        }

        public double Evaluate()
        {
            double result = 0.0;

            if (value is UnaryExpression)
            {
                result = ((UnaryExpression)value).Evaluate();
            }
            else if (value is Element)
            {
                result = ((Element)value).Value;
            }
            else
            {
                throw new Exception("Invalid unary expression: " + value.GetType().FullName);
            }

            switch (op)
            {
                case '+':
                    return result;

                case '-':
                    return -result;

                case '!':
                    if (!MathPlus.IsNatural(result))
                    {
                        throw new Exception("Cannot calculate faculty of non-natural number");
                    }
                    return MathPlus.Fac((long)result);

                default:
                    return result;
            }
        }
    }
}
