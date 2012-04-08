using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Core
{
    public class MathPlus
    {
        public static bool IsNatural(double x)
        {
            return (((double)x - (long)x) == 0.0);
        }

        public static long Fac(long x)
        {
            if (x < 0)
            {
                throw new Exception("Cannot calculate faculty of a number less than zero");
            }

            if (x > 1)
            {
                return x * Fac(x - 1);
            }
            else
            {
                return 1;
            }
        }
    }
}
