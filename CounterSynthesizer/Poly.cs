using System;

namespace CounterSynthesizer
{
    public sealed class Poly
    {
        public Poly(Tuple<string, string> key, KarnaughMap karnaughMap)
        {
            Key = key;
            KarnaughMap = karnaughMap;
        }


        public Tuple<string, string> Key { get; private set; }
        public KarnaughMap KarnaughMap { get; private set; }


        public bool Contains(Poly poly)
        {
            if (KarnaughMap.AreKeysEquale(poly.Key.Item1, Key.Item1) &&
                KarnaughMap.AreKeysEquale(poly.Key.Item2, Key.Item2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public byte[] GetBits()
        {
            byte[] bits = new byte[KarnaughMap.TriggersCount];
            string bitsStr = Key.Item1 + Key.Item2;

            for (int i = 0; i < bitsStr.Length; i++)
                bits[i] = Convert.ToByte(bitsStr[i].ToString());

            return bits;
        }


        public override string ToString()
        {
            return string.Format("{0} {1}", Key.Item1, Key.Item2);
        }
    }
}