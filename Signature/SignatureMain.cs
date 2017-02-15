using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signature
{
    public class SignatureMain
    {      

        static void Main(string[] args)
        {
            string filePath;
            do
            {
                Console.WriteLine("Введите путь к файлу, сигнатуру которого необходимо получить");
                filePath = Console.ReadLine();
            } while (!IsFilePathCorrect(filePath));

            Console.WriteLine("");
            Console.WriteLine("Нажмите любую клавишу для завершения...");
            Console.ReadKey(true);
        }

        public static bool IsFilePathCorrect(String filePath)
        {
            if (filePath.Length < 1)
            {
                Console.WriteLine("Путь к файлу не может быть пустым, повторите ввод");
                return false;
            }

            FileInfo info = new FileInfo(filePath);
            if (!info.Exists)
            {
                Console.WriteLine("Указанный файл не найден, повторите ввод");
                return false;
            }
            if (info.Length == 0)
            {
                Console.WriteLine("Указанный файл пуст, повторите ввод");
                return false;
            }

            return true;

        }
    }
}
