using System;
using System.Collections.Generic;

namespace CounterSynthesizer.Functions
{
    public sealed class Nand : Func
    {
        public Nand()
        {
            Funcs = new List<Func>();
        }

        public Nand(List<Func> funcs)
        {
            Funcs = funcs;
        }



        public List<Func> Funcs { get; private set; }



        public Func Invert()
        {
            And and = new And(Funcs);
            return and;
        }


        public bool ContainsOnly(Type type)
        {
            for (int i = 0; i < Funcs.Count; i++)
            {
                if (Funcs[i].GetType() != type)
                {
                    return false;
                }
            }
            return true;
        }


        public void Add(Func func)
        {
            Funcs.Add(func);
        }


        public override string ToString()
        {
            string str = "-(";

            if (Funcs.Count == 1)
            {
                str = string.Format("{0}{1})",str, Funcs[0]);
            }
            else if (Funcs.Count > 1)
            {
                for (int i = 0; i < Funcs.Count; i++)
                {
                    if (i == 0)
                    {
                        str = string.Format("{0}{1} & ", str, Funcs[i].ToString());
                    }
                    else if (i > 0 && i < Funcs.Count - 1)
                    {
                        str = string.Format("{0}{1} & ", str, Funcs[i].ToString());
                    }
                    else if (i == Funcs.Count - 1)
                    {
                        str = string.Format("{0}{1})", str, Funcs[i].ToString());
                    }
                }
            }
            return str;
        }
    }
}
