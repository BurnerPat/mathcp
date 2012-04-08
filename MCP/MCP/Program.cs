using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;

namespace MCP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MCP - Mathematical Command Prompt [Version 0.5a]");
            Console.WriteLine("Copyright (c) 2012 - All rights reserved.\n");
            Console.WriteLine("Use the 'help' command to see a list of available commands");
            Console.WriteLine("==========================================================\n");

            Core.MCParser parser = new Core.MCParser();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            while (true)
            {
                Console.Write("mcp#> ");
                string input = Console.ReadLine();

                if (input.ToUpper() == "EXIT")
                {
                    break;
                }

                Console.WriteLine(parser.Execute(input) + "\n");
            }
        }
    }
}
