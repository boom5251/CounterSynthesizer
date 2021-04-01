using System;
using System.Collections.Generic;

namespace CounterSynthesizer.Functions
{
    public sealed class And : Func
    {
        public And(List<Func> funcs)
        {
            Funcs = funcs;
        }



        public List<Func> Funcs { get; private set; }



        public Func Invert()
        {
            Nand nand = new Nand(Funcs);
            return nand;
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


        public Nor ToNor()
        {
            Nor nor = new Nor();

            if (this.ContainsOnly(typeof(LogicVar)))
            {
                for (int i = 0; i < Funcs.Count; i++)
                {
                    nor.Add(Funcs[i].Invert());
                }            
            }
            return nor;
        }


        public override string ToString()
        {
            string str = "(";

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
                        str = string.Format("{0}{1} & ", str, Funcs[i].ToString());
                    }
                    else if (i > 0 && i < Funcs.Count - 1)
                    {
                        str = string.Format("{0}{1}& ", str, Funcs[i].ToString());
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
