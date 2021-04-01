using System;
using System.Collections.Generic;

namespace CounterSynthesizer.Functions
{
    public sealed class Or : Func
    {
        public Or()
        {
            Funcs = new List<Func>();
        }

        public Or(List<Func> funcs)
        {
            Funcs = funcs;
        }
        


        public List<Func> Funcs { get; private set; }



        public Func Invert()
        {
            Nor nor = new Nor(Funcs);
            return nor;
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


        public Nand ToNand()
        {
            Nand nand = new Nand();

            for (int i = 0; i < Funcs.Count; i++)
            {
                if ((Funcs[i].GetType() == typeof(And) && 
                    Funcs[i].ContainsOnly(typeof(LogicVar))) ||
                    Funcs[i].GetType() == typeof(LogicVar))
                {
                    nand.Add(Funcs[i].Invert());
                }
            }
            return nand;
        }


        public Nor ToNor()
        {
            Nor nor1 = new Nor();

            for (int i = 0; i < Funcs.Count; i++)
            {
                if (Funcs[i].GetType() == typeof(And))
                {
                    Nor childNor = ((And)Funcs[i]).ToNor();
                    nor1.Add(childNor);
                }
                else if (Funcs[i].GetType() == typeof(LogicVar))
                {
                    nor1.Add(Funcs[i]);
                }
            }
            Nor nor2 = new Nor();
            nor2.Add(nor1);
            return nor2;
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
                        str = string.Format("{0}{1} | ", str, Funcs[i].ToString());
                    }
                    else if (i > 0 && i < Funcs.Count - 1)
                    {
                        str = string.Format("{0}{1}| ", str, Funcs[i].ToString());
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
