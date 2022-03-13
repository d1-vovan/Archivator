using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HuffmanFileWork;

namespace Huffman
{
    class HuffmanTree
    {
        public char ch { get; private set; }
        public double freq { get; private set; }
        public bool isTerminal { get; private set; }
        public HuffmanTree left {get; private set;}
        public HuffmanTree rigth {get; private set;}
        public HuffmanTree(char c, double frequency)
        {
            ch = c;
            freq = frequency;
            isTerminal = true;
            left = rigth = null;
        }
        public HuffmanTree(HuffmanTree l, HuffmanTree r)
        {
            freq = l.freq + r.freq;
            isTerminal = false;
            left = l;
            rigth = r;
        }
    }
    class HuffmanInfo
    {
		HuffmanTree Tree; // дерево кода Хаффмана, потребуется для распаковки
        Dictionary<char,string> Table; // словарь, хранящий коды всех символов, будет удобен для сжатия
        public HuffmanInfo(string fileName)
        {   
			string line;		
            StreamReader sr = new StreamReader(fileName, Encoding.Unicode);
            // считать информацию о частотах символов
            List<HuffmanTree> freqOfOccur = new List<HuffmanTree>();

            while ((line = sr.ReadLine()) != null)
			{
				if (line.Length == 0)
				{
                    //TODO: отдельная обработка строки, которой соответствует частота символа "конец строки" 
                    line = sr.ReadLine();
                    freqOfOccur.Add(new HuffmanTree('\n', Convert.ToDouble(line.Substring(1))));
                }
				else
				{
                    //TODO: создаем вершину (лист) дерева с частотой очередного символа
                    freqOfOccur.Add(new HuffmanTree(line[0], Convert.ToDouble(line.Substring(2))));
				}
			}
            sr.Close();
            // TODO: добавить еще одну вершину-лист, соответствующую символу с кодом 0 ('\0'), который будет означать конец файла. Частота такого символа, очевидно, должна быть очень маленькой, т.к. такой символ встречается только 1 раз во всем файле (можно даже сделать частоту = 0)
            freqOfOccur.Add(new HuffmanTree('\0', 0));
            // TODO: построить дерево кода Хаффмана путем последовательного объединения листьев
            while (freqOfOccur.Count > 1)
            {
                Tuple<int, int> mins = TwoMinEl(freqOfOccur);
                HuffmanTree main = new HuffmanTree(freqOfOccur[mins.Item1], freqOfOccur[mins.Item2]);

                freqOfOccur.RemoveAt(Math.Max(mins.Item1, mins.Item2));
                freqOfOccur.RemoveAt(Math.Min(mins.Item1, mins.Item2));
                freqOfOccur.Add(main);
            }
            // Tree = ...
            Tree = freqOfOccur[0];
            // TODO: заполнить таблицу кодирования Table на основе обхода построенного дерева
            Table = PreOrder(Tree, new Dictionary<char, string>(), "");
            /*//*/
        }

        public Tuple<int, int> TwoMinEl(List<HuffmanTree> foo)
        {
            int ind1, ind2;
            double min1, min2;
            if (foo[0].freq < foo[1].freq)
            {
                min1 = foo[0].freq;
                ind1 = 0;
                min2 = foo[1].freq;
                ind2 = 1;
            }
            else
            {
                min1 = foo[1].freq;
                ind1 = 1;
                min2 = foo[0].freq;
                ind2 = 0;
            }
            for (int i = 2; i < foo.Count; i++)
                if (foo[i].freq < min2)
                    if (foo[i].freq < min1)
                    {
                        min2 = min1;
                        ind2 = ind1;
                        min1 = foo[i].freq;
                        ind1 = i;
                    }
                    else
                    {
                        min2 = foo[i].freq;
                        ind2 = i;
                    }

            return new Tuple<int, int>(ind1, ind2);
        }

        public Dictionary<char, string> PreOrder(HuffmanTree tree, Dictionary<char, string> table, string code)
        {
            // Центр-лево-право
            if (tree.isTerminal)
                table.Add(tree.ch, code);
            if (tree.left != null)
                PreOrder(tree.left, table, code + "0");
            if (tree.rigth != null)
                PreOrder(tree.rigth, table, code + "1");

            return table;
        }

        public void Compress(string inpFile, string outFile)
        {
            var sr = new StreamReader(inpFile, Encoding.Unicode);
            var sw = new ArchWriter(outFile); //нужна побитовая запись, поэтому использовать StreamWriter напрямую нельзя
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                // TODO: посимвольно обрабатываем строку, кодируем, пишем в sw
                string code = "", el = "";
                for (int i = 0; i < line.Length; i++)
                {
                    Table.TryGetValue(line[i], out el);
                    code += el;
                }
                Table.TryGetValue('\n', out el); // записываем признак конца строки
                code += el;
                sw.WriteWord(code);
            }
            sr.Close();
            sw.WriteWord(Table['\0']); // записываем признак конца файла
            sw.Finish();
        }
        
        public void Decompress(string archFile, string txtFile)
        {
            var sr = new ArchReader(archFile); // нужно побитовое чтение
            var sw = new StreamWriter(txtFile, false, Encoding.Unicode);
            byte curBit;
            HuffmanTree pTree = Tree;
            while (sr.ReadBit(out curBit))
            {
                // TODO: побитово (!) разбираем архив
                if (curBit == 1)
                {
                    pTree = pTree.rigth;
                }
                else
                {
                    pTree = pTree.left;
                }
                if (pTree.isTerminal)
                {
                    if (pTree.ch == '\0')
                    {
                        break;
                    }
                    else if (pTree.ch == '\n')
                    {
                        sw.Write('\r');
                    }
                    sw.Write(pTree.ch);
                    pTree = Tree;
                }
            }
            sr.Finish();
            sw.Close();
        }
    }
}