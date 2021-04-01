using CounterSynthesizer.Functions;
using System.Collections.Generic;

namespace CounterSynthesizer
{
    public sealed class FunctionConverter
    {
        public static string ConvertToNand(List<Poly> polies)
        {
            if (polies != null && polies.Count > 0)
            {
                Or or = GetFunc(polies);

                Nand nand = or.ToNand();
                return nand.ToString();
            }
            else
            {
                return null;
            }
        }



        public static string ConvertToNor(List<Poly> polies)
        {
            if (polies != null && polies.Count > 0)
            {
                Or or = GetFunc(polies);

                Nor nor = or.ToNor();

                if (nor.Funcs.Count == 1 && ((Nor)nor.Funcs[0]).Funcs.Count == 0)
                {
                    return "1";
                }
                else
                {
                    return nor.ToString();
                }
            }
            else
            {
                return null;
            }
        }



        private static Or GetFunc(List<Poly> polies)
        {
            Or or = new Or();

            for (int i = 0; i < polies.Count; i++)
            {
                List<Func> logicVars = new List<Func>();
                byte[] values = polies[i].GetBits();

                for (int j = 0; j < values.Length; j++)
                {
                    if (values[j] == 0 || values[j] == 1)
                    {
                        byte index = (byte)(values.Length - j - 1);
                        logicVars.Add(new LogicVar(values[j], index));
                    }
                }

                if (logicVars.Count == 1)
                {
                    or.Add(logicVars[0]);
                }
                else if (logicVars.Count > 1)
                {
                    And and = new And(logicVars);
                    or.Add(and);
                }
            }
            return or;
        }
    }
}
