using System;
using System.Collections.Generic;

namespace CounterSynthesizer
{
    public sealed class KarnaughMap
    {
        public KarnaughMap(List<string> binaryStrings, List<byte> triggerColumn, int triggersCount)
        {
            BinaryStrings = binaryStrings;
            ValuesTable = triggerColumn;

            TriggersCount = triggersCount;

            Rows = (int)Math.Pow(2, triggersCount / 2);
            int remnant = triggersCount % 2;
            Columns = (int)Math.Pow(2, triggersCount / 2 + remnant);

            Polies = new List<Poly>();

            SetKeys();
            FillMap();
        }



        public List<string> BinaryStrings { get; }
        public List<byte> ValuesTable { get; }

        private Tuple<List<string>, List<string>> Keys { get; set; }
        private byte[,] Map { get; set; }
        private List<Poly> Polies { get; set; }

        public int Rows { get; }
        public int Columns { get; }
        public int TriggersCount { get; }



        private void SetKeys()
        {
            Keys = new Tuple<List<string>, List<string>>
            (
            new List<string>() { "0", "1" }, // Горизонтальные ключи (сверху).
            new List<string>() { "0", "1" } // Вертикальные ключи (слева).
            );

            for (int i = 2; i < TriggersCount; i++)
            {
                /// <summary>
                /// Первый вложенный цикл добавляет биты, отображенные зеркально.
                /// Второй вложенный цикл добавляет старший разряд: 0 к изначально имеющимся битам и 1 к отображенным зеркально.
                /// Пример выполнения этих двух циклов:
                ///                 0011
                /// 01 >>> 0110 >>> 0110
                /// </summary>

                List<string> currentList;
                int remainder = i % 2;

                if (remainder == 0)
                    currentList = Keys.Item1;
                else if (remainder == 1)
                    currentList = Keys.Item2;
                else
                    currentList = new List<string>();

                for (int j = currentList.Count - 1; j >= 0; j--)
                {
                    currentList.Add(currentList[j]);
                }

                for (int j = 0; j < currentList.Count; j++)
                {
                    if (j < currentList.Count / 2)
                        currentList[j] = "0" + currentList[j];
                    else
                        currentList[j] = "1" + currentList[j];
                }
            }
        }



        // Данный метод преобразует список щначений переменных в длюч
        private Tuple<string, string> GetKey(List<byte> keyValues)
        {
            string colBits = string.Empty;
            string rowBits = string.Empty;

            // Разделение битов на вертикальные (старшие, 0) и горизонтальные (младшие, 1).
            for (int i = 0; i < TriggersCount; i++)
            {
                if (i < Math.Log(Columns, 2))
                {
                    colBits += Convert.ToString(keyValues[i]);
                }
                else
                {
                    rowBits += Convert.ToString(keyValues[i]);
                }
            }

            Tuple<string, string> key = new Tuple<string, string>(colBits, rowBits);
            return key;
        }



        public List<Poly> Minimize()
        {
            /// <summary>
            /// Количество вариаций ключей: 3^TriggersCount.
            /// 3 - количество возможых значений (0, 1, 2).
            /// </summary>
            int num = (int)Math.Pow(3, TriggersCount) - 1;

            List<string> valuesStrs = new List<string>();
            List<byte> currentValues;

            while (num > 0) // Создание списка строк значений.
            {
                valuesStrs.Add(ConvertToBase3(num));
                num--;
            }

            valuesStrs = Sort(valuesStrs); // Сортировка списка строк значений. 

            for (int i = 0; i < valuesStrs.Count; i++)
            {
                currentValues = new List<byte>();

                for (int j = 0; j < TriggersCount; j++)
                {
                    currentValues.Add(byte.Parse(valuesStrs[i][j].ToString())); // Создание списка значений триггеров
                }

                Tuple<string, string> key = GetKey(currentValues); // Полученик ключа.

                if (CanMinimizeSection(key, out Poly poly))
                {
                    if (!IsInPoliesList(poly))
                    {
                        // Добавление полигона в список при условии, что он не содержится в уже добавленных полигонов.
                        Polies.Add(poly); 
                    }
                }
            }       
            if (Polies.Count > 0)
            {
                return Polies;
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// Данный метод создает строку значений.
        /// Длина этой строки равна количеству триггеров.
        /// Если длина строки меньше количества триггеров, то к ней добавляются незначимые числа ('2').
        /// </summary>
        private string ConvertToBase3(int num)
        {
            string str = string.Empty;
            int quotient = num;
            int remainder;

            while (quotient > 2)
            {
                remainder = quotient % 3;
                quotient /= 3;

                str = string.Format("{0}{1}", remainder, str);
            }

            str = string.Format("{0}{1}", quotient, str);

            if (str.Length < TriggersCount)
            {
                int lenght = TriggersCount - str.Length;

                for (int i = 0; i < lenght; i++)
                {
                    str = string.Format("0{0}", str);
                }
            }
            return str;
        }


        // Данный метод сортирует список строк по количеству незначимых полей ('2').
        private List<string> Sort(List<string> valuesStrsList)
        {
            string temp;

            for (int i = 0; i < valuesStrsList.Count; i++)
            {
                for (int j = i + 1; j < valuesStrsList.Count; j++)
                {
                    if (GetEmptyBitsCount(valuesStrsList[i]) < GetEmptyBitsCount(valuesStrsList[j]))
                    {
                        temp = valuesStrsList[j];
                        valuesStrsList[j] = valuesStrsList[i];
                        valuesStrsList[i] = temp;
                    }
                }
            }
            return valuesStrsList;
        }



        // Данный метод возвращает количество довек ('2') в строке.
        public int GetEmptyBitsCount(string valuesStr)
        {
            int count = 0;

            for (int i = 0; i < valuesStr.Length; i++)
            {
                if (valuesStr[i] == '2')
                {
                    count++;
                }
            }
            return count;
        }



        // Данный метод проверяет, можно ли минимизировать передаваемую часть карты.
        private bool CanMinimizeSection(Tuple<string, string> key, out Poly poly)
        {
            poly = null;

            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    string keyStrFromList = Keys.Item1[i] + Keys.Item2[j];
                    string currentKeyStr = key.Item1 + key.Item2;

                    if (AreKeysEquale(keyStrFromList, currentKeyStr))
                    {
                        if (Map[i, j] == 0)
                        {
                            return false;
                        }
                    }
                }
            }

            if (!IsKeyEmpty(key))
            {
                poly = new Poly(key, this);
                return true;
            }
            else
            {
                return false;
            }
        }


        
        // Данный метод проверяет, являются ли все значения данного ключа запрещенными.
        private bool IsKeyEmpty(Tuple<string, string> key)
        {
            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    string keyStrFromList = Keys.Item1[i] + Keys.Item2[j];
                    string currentKeyStr = key.Item1 + key.Item2;

                    if (AreKeysEquale(keyStrFromList, currentKeyStr))
                    {
                        if (Map[i, j] != 2 && Map[i, j] != 3)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }



        // Данный метод проверяет равество ключей с учетом незадействованных переменных ('2').
        public bool AreKeysEquale(string keyStr1, string keyStr2)
        {
            for (int i = 0; i < keyStr1.Length; i++)
            {
                if (keyStr1[i] != keyStr2[i] && keyStr2[i] != '2')
                {
                    return false;
                }
            }
            return true;
        }



        /// <summary>
        /// Данный метод проверяет, содержится ли конкретный полигон в одном или в нескольких полигонах в списке карты.
        /// Если данный полигон не содержится ни в одном полигоне карты, то он добавляется в список.
        /// Полигоны представляют собой части карты и позволяют минимизировать логическую функцию.
        /// </summary>
        private bool IsInPoliesList(Poly poly)
        {
            for (int i = 0; i < Polies.Count; i++)
            {
                if (Polies[i].Contains(poly))
                {
                    return true;
                } 
            }
            return false;
        }



        // Данный метод заполняет карту значениями.
        private void FillMap()
        {
            Map = new byte[Columns, Rows];

            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    // Заполнение для последующих запрещенных состояний (3 - запрещенное состояние).
                    Map[i, j] = 3;
                }
            }
            
            for (int i = 0; i < BinaryStrings.Count; i++)
            {
                string binStr = BinaryStrings[i].Substring(8 - TriggersCount, 8 - (8 - TriggersCount));

                string columnBits = binStr.Substring(0, (int)Math.Log(Columns, 2));
                string rowsBits = binStr.Substring((int)Math.Log(Columns, 2));

                int columnIndex = Keys.Item1.IndexOf(columnBits);
                int rowIndex = Keys.Item2.IndexOf(rowsBits);

                Map[columnIndex, rowIndex] = ValuesTable[i];
            }
        }



        // Данный метод отображает карту на консоли.
        public void ShowMap()
        {
            for (int j = 0; j < Rows; j++)
            {
                Console.WriteLine();

                for (int i = 0; i < Columns; i++)
                {
                    string value;

                    if (Map[i, j] == 2) 
                    {
                        value = "a";
                    }
                    else if (Map[i, j] == 3)
                    {
                        value = "x";
                    }
                    else
                    {
                        value = Map[i, j].ToString();
                    }
                    Console.Write(string.Format("{0} ", value));
                }
            }
        }
    }
}
