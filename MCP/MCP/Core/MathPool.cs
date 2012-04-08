using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Core
{
    class MathPool
    {
        public static bool IsMathFunction(string str)
        {
            switch (str.ToLower())
            {
                case "sin":
                case "cos":
                case "tan":
                case "asin":
                case "acos":
                case "atan":
                case "atan2":
                case "sqr":
                case "sqrt":
                case "abs":
                    return true;

                default:
                    return false;
            }
        }

        public static double Execute(MathFunction func)
        {
            switch (func.Parameters.Count)
            {
                case 0:
                    return Execute0P(func.Name);

                case 1:
                    return Execute1P(func.Name, func.Parameters[0].Evaluate());

                case 2:
                    return Execute2P(func.Name, func.Parameters[0].Evaluate(), func.Parameters[1].Evaluate());

                default:
                    throw new Exception("Unknown function call \"" + func.Name + "\"");
            }
        }

        private static double Execute0P(string name)
        {
            switch (name.ToLower())
            {
                default:
                    throw new Exception("Unknown function call \"" + name + "\" with 0 parameters");
            }
        }

        private static double Execute1P(string name, double x1)
        {
            switch (name.ToLower())
            {
                case "sin":
                    return Math.Sin(x1);

                case "cos":
                    return Math.Cos(x1);

                case "tan":
                    return Math.Tan(x1);

                case "asin":
                    return Math.Asin(x1);

                case "acos":
                    return Math.Acos(x1);

                case "atan":
                    return Math.Atan(x1);

                case "sqrt":
                    return Math.Sqrt(x1);

                case "sqr":
                    return x1 * x1;

                case "abs":
                    return Math.Abs(x1);

                default:
                    throw new Exception("Unknown function call \"" + name + "\" with 1 parameter");
            }
        }

        private static double Execute2P(string name, double x1, double x2)
        {
            switch (name.ToLower())
            {
                default:
                    throw new Exception("Unknown function call \"" + name + "\" with 2 parameters");
            }
        }
    }
}
