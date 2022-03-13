using Huffman;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists("freq.txt"))
            {
                Console.WriteLine("Не найден файл с частотами символов!");
                return;
            }
            if (args.Length == 0)
            {
                var hi = new HuffmanInfo("freq.txt");
                hi.Compress("etalon2.txt", "etalon2.arc");
                hi.Decompress("etalon2.arc", "etalon_dec2.txt");
                return;
            }
            if (args.Length != 3 || args[0] != "zip" && args[0] != "unzip")
            {
                Console.WriteLine("Синтаксис:");
                Console.WriteLine("Huffman.exe zip <имя исходного файла> <имя файла для архива>");
                Console.WriteLine("либо");
                Console.WriteLine("Huffman.exe unzip <имя файла с архивом> <имя файла для текста>");
                Console.WriteLine("Пример:");
                Console.WriteLine("Huffman.exe zip text.txt text.arc");
                return;
            }
            var HI = new HuffmanInfo("freq.txt");
            if (args[0] == "zip")
            {
                if (!File.Exists(args[1]))
                {
                    Console.WriteLine("Не найден файл с исходным текстом!");
                    return;
                }
                HI.Compress(args[1], args[2]);
            }
            else
            {
                if (!File.Exists(args[1]))
                {
                    Console.WriteLine("Не найден файл с архивом!");
                    return;
                }
                HI.Decompress(args[1], args[2]);
            }
            Console.WriteLine("Операция успешно завершена!");
            Console.ReadKey();
        }
    }
}
