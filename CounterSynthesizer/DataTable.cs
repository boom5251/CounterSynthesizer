using System;
using System.Collections.Generic;

namespace CounterSynthesizer
{
    public sealed class DataTable
    {
        public DataTable(List<string> binaryStrings, int triggersCount)
        {
            int rowsCount = binaryStrings.Count;
            
            OldTable = new byte[rowsCount, triggersCount];
            NewTable = new byte[rowsCount, triggersCount];

            FillTables(rowsCount, triggersCount, binaryStrings);

            DTriggerTable = new List<byte>[triggersCount];
            JKTriggerTable = new List<Tuple<byte, byte>>[triggersCount];

            for (int i = 0; i < triggersCount; i++)
            {
                DTriggerTable[i] = new List<byte>();
                JKTriggerTable[i] = new List<Tuple<byte, byte>>();
            }

            FillTriggersTables(rowsCount, triggersCount);
        }

        

        public byte[,] OldTable { get; private set; } // Строка матрицы - строка таблицы переходов.
        public byte[,] NewTable { get; private set; }

        /// <summary>
        /// Элементы следующих массивов - столбцы значений входов триггеров.
        /// Например: нулевой элемент массива - столбец (список) всех значений, поданых на вход нулевого триггера.
        /// </summary>
        public List<byte>[] DTriggerTable { get; private set; }
        public List<Tuple<byte, byte>>[] JKTriggerTable { get; private set; }



        private void FillTables(int rowsCount, int triggersCount, List<string> binaryStrings)
        {
            for (int j = 0; j < triggersCount; j++)
            {
                for (int i = 0; i < rowsCount; i++) // Заполнение таблицы OldTable.
                {
                    int startPos = 8 - triggersCount;
                    int length = 8 - startPos;
                    char[] binaryChars = binaryStrings[i].Substring(startPos, length).ToCharArray();

                    int k = triggersCount - 1 - j; // Запись справа налево (от младшего бита к старшему).

                    OldTable[i, k] = byte.Parse(binaryChars[j].ToString());
                }

                for (int i = 0; i < rowsCount; i++) // Заполнение таблицы NewTable.
                {
                    int k = triggersCount - 1 - j;

                    if (i < rowsCount - 1)
                        NewTable[i, k] = OldTable[i + 1, k];
                    else
                        NewTable[i, k] = OldTable[0, k];
                }
            }
            ShowTranslationTable();
        }

        

        private void FillTriggersTables(int rowsCount, int triggersCount)
        {
            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < triggersCount; j++)
                {
                    byte O, N;

                    O = OldTable[i, j];
                    N = NewTable[i, j];

                    // Для D-Триггера:
                    if (N == 1)
                    {
                        DTriggerTable[j].Add(1);
                    }
                    else if (N == 0)
                    {
                        DTriggerTable[j].Add(0);
                    }
                    else
                    {
                        new Exception("Недопустимое значение!");
                    }

                    // Для JK-Тригерра:
                    byte J, K;

                    if (O == 0 && N == 0)
                    {
                        J = 0;
                        K = 2; // Альфа = 2
                    }
                    else if (O == 0 && N == 1)
                    {
                        J = 1;
                        K = 2;
                    }
                    else if (O == 1 && N == 0)
                    {
                        J = 2;
                        K = 1;
                    }
                    else if (O == 1 && N == 1)
                    {
                        J = 2;
                        K = 0;
                    }
                    else
                    {
                        J = 0;
                        K = 0;
                        new Exception("Недопустимое значение!");
                    }

                    JKTriggerTable[j].Add(new Tuple<byte, byte>(J, K));
                }
            }
        }



        public List<byte> GetDTriggerTableByIndex(int index)
        {
            return DTriggerTable[index];
        }


        public List<Tuple<byte, byte>> GetJKTriggerTableByIndex(int index)
        {
            return JKTriggerTable[index];
        }



        private void ShowTranslationTable()
        {
            Console.WriteLine("Таблицы переходов триггеров:");

            for (int j = 0; j < OldTable.GetLength(1); j++)
            {
                Console.WriteLine(string.Format("Триггер {0}:", j));

                for (int i = 0; i < OldTable.GetLength(0); i++)
                {
                    Console.WriteLine(string.Format("{0}->{1}", OldTable[i, j], NewTable[i, j]));
                }
                Console.WriteLine();
            }
        }
    }
}
