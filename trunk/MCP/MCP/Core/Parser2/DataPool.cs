using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCP.Core.Basic.AltParser
{
    class DataPool
    {
        private static DataPool instance = new DataPool();
        private Dictionary<string, Variable> data = new Dictionary<string, Variable>();
        private List<string> constants = new List<string>();
        private Variable ans = new Variable("ANS", 0.0);

        public DataPool()
        {
            data.Add("PI", new Variable("PI", Math.PI));
            constants.Add("PI");

            data.Add("E", new Variable("E", Math.E));
            constants.Add("E");

            data.Add("ANS", ans);
            constants.Add("ANS");
        }

        public static DataPool GetInstance()
        {
            return instance;
        }

        public Variable Get(string pName)
        {
            string name = pName;

            if (constants.Contains(name.ToUpper()))
            {
                name = name.ToUpper();
            }

            if (data.ContainsKey(name))
            {
                return data[name];
            }
            else
            {
                throw new Exception("\"" + name + "\" could not be resolved - GET failed");
            }
        }

        public void Register(Variable var)
        {
            if (constants.Contains(var.Name.ToUpper()))
            {
                throw new Exception("\"" + var.Name + "\" is a constant and cannot be overwritten");
            }

            if (data.ContainsKey(var.Name))
            {
                throw new Exception("\"" + var.Name + "\" already exists - CREATE failed");
            }
            else
            {
                data.Add(var.Name, var);
            }
        }

        public void Set(string name, Variable value)
        {
            if (constants.Contains(name.ToUpper()))
            {
                throw new Exception("\"" + name + "\" is a constant and cannot be overwritten");
            }

            if (!data.ContainsKey(name))
            {
                throw new Exception("\"" + name + "\" could not be resolved - SET failed");
            }
            else
            {
                data[name] = value;
            }
        }

        public void Clear()
        {
            data.Clear();

            data.Add("PI", new Variable("PI", Math.PI));
            data.Add("E", new Variable("E", Math.E));
            data.Add("ANS", ans);
        }

        public bool Contains(string str)
        {
            return data.ContainsKey(str);
        }

        public void Ans(double x)
        {
            ans.Value = x;
        }

        public string GenerateTable()
        {
            if (data.Count == 0)
            {
                return null;
            }

            int nameCol = ("Variable").Length;
            int valCol = ("Value").Length;

            List<string> names = new List<string>();
            List<string> values = new List<string>();

            foreach (KeyValuePair<string, Variable> var in data)
            {
                if (var.Value is Variable && !constants.Contains(var.Key) && !(var.Value is Function))
                {
                    string name = var.Key;
                    string value = var.Value.Value.ToString();

                    if (name.Length > nameCol)
                    {
                        nameCol = name.Length;
                    }
                    if (value.Length > valCol)
                    {
                        valCol = value.Length;
                    }

                    names.Add(name);
                    values.Add(value);
                }
            }

            if (names.Count == 0)
            {
                return null;
            }

            String line = "+-" + Line(nameCol) + "-+-" + Line(valCol) + "-+";
            String result = line + "\n";

            result += "| " + AlignLeft("Variable", nameCol) + " | " + AlignLeft("Value", valCol) + " |\n";
            result += line + "\n";

            List<string> table = new List<string>();

            for (int i = 0; i < names.Count; i++)
            {
                table.Add("| " + AlignLeft(names[i], nameCol) + " | " + AlignRight(values[i], valCol) + " |\n");
            }

            table.Sort();

            for (int i = 0; i < table.Count; i++)
            {
                result += table[i];
            }

            result += line;
            return result;
        }

        public string GenerateFunctionTable()
        {
            if (data.Count == 0)
            {
                return null;
            }

            int nameCol = ("Name").Length;
            int valCol = ("Function").Length;
            int parCol = ("Parameters").Length;

            List<string> names = new List<string>();
            List<string> values = new List<string>();
            List<string> parameters = new List<string>();

            foreach (KeyValuePair<string, Variable> var in data)
            {
                if (var.Value is Function)
                {
                    string name = var.Key;
                    string value = ((Function)var.Value).TokenString();

                    string pars = "";

                    if (((Function)var.Value).Parameters.Count > 0)
                    {
                        pars += ((Function)var.Value).Parameters[0].Name;
                    }

                    for (int i = 1; i < ((Function)var.Value).Parameters.Count; i++)
                    {
                        pars += ", " + ((Function)var.Value).Parameters[i].Name;
                    }

                    if (name.Length > nameCol)
                    {
                        nameCol = name.Length;
                    }
                    if (value.Length > valCol)
                    {
                        valCol = value.Length;
                    }
                    if (pars.Length > parCol)
                    {
                        parCol = pars.Length;
                    }

                    names.Add(name);
                    values.Add(value);
                    parameters.Add(pars);
                }
            }

            if (names.Count == 0)
            {
                return null;
            }

            String line = "+-" + Line(nameCol) + "-+-" + Line(parCol) + "-+-" + Line(valCol) + "-+";
            String result = line + "\n";

            result += "| " + AlignLeft("Name", nameCol) + " | " + AlignLeft("Parameters", parCol) + " | " + AlignLeft("Function", valCol) + " |\n";
            result += line + "\n";

            List<string> table = new List<string>();

            for (int i = 0; i < names.Count; i++)
            {
                table.Add("| " + AlignLeft(names[i], nameCol) + " | " + AlignLeft(parameters[i], parCol) + " | " + AlignRight(values[i], valCol) + " |\n");
            }

            table.Sort();

            for (int i = 0; i < table.Count; i++)
            {
                result += table[i];
            }

            result += line;
            return result;
        }

        public string Line(int width)
        {
            string result = "";

            for (int i = 0; i < width; i++)
            {
                result += "-";
            }

            return result;
        }

        public string AlignLeft(string str, int width)
        {
            string result = str;

            while (result.Length < width)
            {
                result += " ";
            }

            return result;
        }

        public string AlignRight(string str, int width)
        {
            string result = str;

            while (result.Length < width)
            {
                result = " " + result;
            }

            return result;
        }
    }
}
