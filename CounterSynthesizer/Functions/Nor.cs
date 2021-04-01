using System;
using System.Collections.Generic;

namespace CounterSynthesizer.Functions
{
    public sealed class Nor : Func
    {
        public Nor()
        {
            Funcs = new List<Func>();
        }

        public Nor(List<Func> funcs)
        {
            Funcs = funcs;
        }



        public List<Func> Funcs { get; private set; }

        

        public Func Invert()
        {
            Or or = new Or(Funcs);
            return or;
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
                str = string.Format("{0}{1})", str, Funcs[0]);
            }
            else if (Funcs.Count > 1)
            {
                for (int i = 0; i < Funcs.Count; i++)
                {
                    if (i == 0)
                    {
                        str = string.Format("{0}{1} | ", str, Funcs[i].ToString());
                    }
                    else if (i > 0 && i < Funcs.Count - 1)
                    {
                        str = string.Format("{0}{1} | ", str, Funcs[i].ToString());
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
