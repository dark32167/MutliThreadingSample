using System;
using System.IO;

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

            uint lenghtBlocks;
            string tmpLenghtBlocks;
            do
            {
                Console.WriteLine("Введите длинну блоков в байтах.");
                Console.WriteLine("Это должно быть целое положительное число.");
                tmpLenghtBlocks = Console.ReadLine();
            } while (!IsLenghtBlocksCorrect(tmpLenghtBlocks));

            lenghtBlocks = uint.Parse(tmpLenghtBlocks);

            Signature signature = new Signature(filePath, lenghtBlocks);
            signature.StartCalculateSignature();

            Console.WriteLine("");
            Console.WriteLine("Нажмите любую клавишу для завершения...");
            Console.ReadKey(true);
        }

        public static bool IsLenghtBlocksCorrect(string tmpLenghtBlocks)
        {
            if (tmpLenghtBlocks.Length < 1)
            {
                Console.WriteLine("Ввод не может быть пустым.");
                return false;
            }

            int LenghtBlocks;

            try
            {
               LenghtBlocks = int.Parse(tmpLenghtBlocks);
            }
            catch
            {
                Console.WriteLine("Ввод не может содержать символы отличные от цифр и быть больше " + int.MaxValue);
                return false;
            }

            if (LenghtBlocks < 1)
            {
                Console.WriteLine("Длинна блоков должна быть больше 0");
                return false;
            }

            return true;
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
