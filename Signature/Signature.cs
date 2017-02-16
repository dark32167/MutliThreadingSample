using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Signature
{
    class Signature
    {
        private string filePath;
        private int lenghtBlocks;
        private long countBlocksInFile;
        private Int64 countTasks;        
        
        public Signature(string filePath, int lenghtBlocks)
        {
            this.filePath = filePath;
            this.lenghtBlocks = lenghtBlocks;
            FileInfo fileInfo = new FileInfo(filePath);
            this.countBlocksInFile = CalculateCountOfBlocksInFile(fileInfo.Length);
            
            PerformanceCounter _ramCounter = new PerformanceCounter("Memory", "Available Bytes");
            Int64 AvailableRam = (Int64)_ramCounter.NextValue();
            countTasks = AvailableRam / lenghtBlocks;

            /* В ходе интеграционых тестов выяснилось что слишком большое количество тасков не увеличивает производительность,
             * но если в файле больше блоков чем возможно создать в оперативной памяти не срабатывает Task.WaitAll(tasks) 
             * из-за не определенных элементов массива
             */
            if (countTasks > countBlocksInFile)
                countTasks = countBlocksInFile;
        }

        public void StartCalculateSignature()
        {
            System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
            myStopwatch.Start();
         
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    /* Не получилось добиться прироста производительности от использования thread
                     * в интернете почти все описания thread переходят в то, что сейчас уже есть более удобные 
                     * и безопасные инструменты для выполнения параллельных вычислений...
                     */
                    Task[] tasks = new Task[countTasks];

                    for (long i = 0; i < countBlocksInFile; i++)
                    {
                        long currentTaskNumber = i % countTasks;

                        if (i > countTasks)
                            tasks[currentTaskNumber].Wait();

                       tasks[currentTaskNumber] = new Task(CalculateSHA256Async,new HashingBlock(binaryReader.ReadBytes(lenghtBlocks), i + 1));
                       tasks[currentTaskNumber].Start();
                    }

                    Task.WaitAll(tasks);
                };
            };

            myStopwatch.Stop();
            Console.WriteLine("Работа завершена, время выполнения: {0}", myStopwatch.Elapsed);
        }

        private void CalculateSHA256Async(object objHashingBlock)
        {
            HashingBlock hashingBlock = (HashingBlock)objHashingBlock;
            SHA256 hash = SHA256Managed.Create();
            byte[] hashValue = hash.ComputeHash(hashingBlock.contentForHash);

            hashingBlock.contentForHash = null;

            /*В ходе интеграционных тестов выяснилось, 
             *что при очень большом значении lenghtBlocks GC не успевает сработать автоматически 
             *и программа выкидывает outOfMemoryException
             */
            GC.Collect();

            string hashString = "";
            foreach (var item in hashValue)
            {
                hashString += String.Format("{0:X2}", item);
            }
            Console.WriteLine("[{0}] из [{1}] " + hashString , hashingBlock.curentBlockNumber, countBlocksInFile);
        }

        private long CalculateCountOfBlocksInFile(long lengthFile)
        {
            if (lengthFile % lenghtBlocks == 0)
                return lengthFile / lenghtBlocks;
            else
                return lengthFile / lenghtBlocks + 1;
        }

        private class HashingBlock
        {
            public byte[] contentForHash;
            public long curentBlockNumber;

            public HashingBlock(byte[] contentForHash, long curentBlockNumber)
            {
                this.contentForHash = contentForHash;
                this.curentBlockNumber = curentBlockNumber;
            }
        }
    }
}
