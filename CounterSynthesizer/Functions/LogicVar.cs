using System;

namespace CounterSynthesizer.Functions
{
    public sealed class LogicVar : Func
    {
        public LogicVar(byte val, byte index)
        {
            if (val == 0 || val == 1)
            {
                Value = val;
                Index = index;
            }
        }



        public byte Value { get; set; }
        public byte Index { get; private set; }



        public Func Invert()
        {
            LogicVar invertedLogicVar = null;

            if (Value == 0)
            {
                invertedLogicVar = new LogicVar(1, Index);
            }
            else if (Value == 1)
            {
                invertedLogicVar = new LogicVar(0, Index);
            }
            return invertedLogicVar;
        }


        public bool ContainsOnly(Type type)
        {
            return true;
        }


        public override string ToString()
        {
            string str = string.Format("X{0}", Index);

            if (Value == 0)
            {
                str = string.Format("-{0}", str);
            }

            return str;
        }
    }
}
