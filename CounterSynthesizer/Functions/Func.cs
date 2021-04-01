using System;

namespace CounterSynthesizer.Functions
{
    public interface Func
    {
        Func Invert();
        bool ContainsOnly(Type type);
    }
}
