using System;
using System.Collections.Generic;
using System.Linq;

namespace CounterSynthesizer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = "ТА. Синтезатор счетчиков. Куликов В.Ю. (ИВБО-06-19)";


            byte max = 0;
            byte step = 0;

            try
            {
                Console.Write("Введите максимальное значение счетчика: ");
                max = byte.Parse(Console.ReadLine());

                if (max <= 2 || max > 255)
                    throw new FormatException();

                Console.Write("Введите шаг счетчика: ");
                step = byte.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Некорректные данные!\n");
                Main(new string[0]);
            }


            Console.WriteLine("\nТаблица истинности счетчика:");
            List<byte> counterByteTable = GetBytesTable(max, step);


            List<string> binaryStrings = GetBinaryStringsTable(counterByteTable);
            
            for (int i = 0; i < binaryStrings.Count; i++)
            {
                string binaryString = string.Format("{0}: {1} = {2}", i, binaryStrings[i], counterByteTable[i]);
                Console.WriteLine(binaryString);
            }
            Console.WriteLine();


            int triggersCount = GetTriggersCount(counterByteTable);
            DataTable transitionTable = new DataTable(binaryStrings, triggersCount);


            string command = GetSchemaType();

            if (command == "NAND_D")
            {
                KarnaughMap[] DMaps = new KarnaughMap[triggersCount];

                for (int i = 0; i < triggersCount; i++)
                {
                    List<byte> DColumn = transitionTable.GetDTriggerTableByIndex(i);
                    DMaps[i] = new KarnaughMap(binaryStrings, DColumn, triggersCount);

                    Console.Write(string.Format("D{0}:", i));
                    DMaps[i].ShowMap();
                    Console.WriteLine();

                    List<Poly> polies = DMaps[i].Minimize();
                    string nandStr = FunctionConverter.ConvertToNand(polies);
                    Console.WriteLine(string.Format("D = {0}", nandStr));

                    Console.WriteLine();
                }
            }
            else if (command == "NOR_JK")
            {
                KarnaughMap[] JMaps = new KarnaughMap[triggersCount];
                KarnaughMap[] KMaps = new KarnaughMap[triggersCount];

                for (int i = 0; i < triggersCount; i++)
                {
                    List<byte> JColumn = new List<byte>();
                    List<byte> KColumn = new List<byte>();

                    List<Tuple<byte, byte>> JKColumn = transitionTable.GetJKTriggerTableByIndex(i);

                    for (int j = 0; j < JKColumn.Count; j++)
                    {
                        JColumn.Add(JKColumn[j].Item1);
                        KColumn.Add(JKColumn[j].Item2);
                    }

                    JMaps[i] = new KarnaughMap(binaryStrings, JColumn, triggersCount);
                    KMaps[i] = new KarnaughMap(binaryStrings, KColumn, triggersCount);


                    Console.Write(string.Format("J{0}", i));
                    JMaps[i].ShowMap();
                    Console.WriteLine();

                    List<Poly> JPolies = JMaps[i].Minimize();
                    string JNorStr = FunctionConverter.ConvertToNor(JPolies);
                    Console.WriteLine(string.Format("J = {0}", JNorStr));


                    Console.Write(string.Format("\nK{0}:", i));
                    KMaps[i].ShowMap();
                    Console.WriteLine();

                    List<Poly> KPolies = KMaps[i].Minimize();
                    string KNorStr = FunctionConverter.ConvertToNor(KPolies);
                    Console.WriteLine(string.Format("K = {0}", KNorStr));
                    Console.WriteLine("\n");
                }
            }
            Console.ReadKey();
        }



        public static List<byte> GetBytesTable(byte max, byte step)
        {
            List<byte> byteTable = new List<byte>() { 0 };

            while(true)
            {
                byte currentByte = (byte)(byteTable.Last() + step);

                if (currentByte >= max)
                {
                    currentByte -= max;
                }

                if (currentByte == byteTable[0])
                {
                    return byteTable;
                }

                byteTable.Add(currentByte);
            }
        }



        public static List<string> GetBinaryStringsTable(List<byte> byteTable)
        {
            List<string> binaryStrings = new List<string>();

            for (int i = 0; i < byteTable.Count; i++)
            {
                string bitRepresentation = Convert.ToString(byteTable[i], 2);

                if (bitRepresentation.Length < 8)
                {
                    int length = 8 - bitRepresentation.Length;

                    for (int j = 0; j < length; j++)
                    {
                        bitRepresentation = "0" + bitRepresentation;
                    }
                }
                binaryStrings.Add(bitRepresentation);
            }
            return binaryStrings;
        }



        public static int GetTriggersCount(List<byte> counterByteTable)
        {
            int maxValue = 0;

            for (int i = 0; i < counterByteTable.Count; i++)
            {
                if (counterByteTable[i] > maxValue)
                {
                    maxValue = counterByteTable[i];
                }
            }

            int iterator = 0;

            while (true)
            {
                if (maxValue < Math.Pow(2, iterator))
                {
                    return iterator;
                }
                iterator++;
            }
        }



        public static string GetSchemaType()
        {
            Console.Write("Введите команду (NAND_D / NOR_JK): ");

            string command = Console.ReadLine().ToUpper();

            if (command == "NAND_D" || command == "NOR_JK")
            {
                return command;
            }
            else
            {
                Console.WriteLine("Неверная команда!");
                return GetSchemaType();
            }
        }
    }
}
