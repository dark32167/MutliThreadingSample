using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

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

            int lengthBlocks;
            string tmpLengthBlocks;
            do
            {
                Console.WriteLine("Введите длинну блоков в байтах.");
                Console.WriteLine("Это должно быть целое положительное число.");
                tmpLengthBlocks = Console.ReadLine();
            } while (!IsLengthBlocksCorrect(tmpLengthBlocks));

            lengthBlocks = int.Parse(tmpLengthBlocks);            
            try
            {
                Signature signature = new Signature(filePath, lengthBlocks);
                signature.StartCalculateSignature();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            Console.WriteLine("");
            Console.WriteLine("Нажмите любую клавишу для завершения...");
            Console.ReadKey(true);
        }

        public static bool IsLengthBlocksCorrect(string tmpLenghtBlocks)
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
               if (LenghtBlocks > int.MaxValue / 2)
                   throw new Exception();
            }
            catch
            {
                /* int.MaxValue/2 - блоки размером примерно в 1Гб, 
                 * в ходе интеграционных тестов выяснилось, 
                 * что если попробовать считывать блоки большего размера 
                 * программа вылетит с ошибкой переполнения памяти, 
                 * даже при явной компиляции под х64.
                 */
                Console.WriteLine("Ввод не может содержать символы отличные от цифр и быть больше " + int.MaxValue/2);
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

            FileInfo info = null;
            try
            {
                info = new FileInfo(filePath);
            }
            catch
            {
                Console.WriteLine("Путь к файлу содержит недопустимые символы, повторите ввод");
                return false;
            }

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
