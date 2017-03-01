using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace Signature
{
    class Signature : IDisposable
    {
        private string filePath;
        private int lenghtBlocks;
        private long countBlocksInFile;
        private long countDoneBlocks = 0;
        private int countThreads = Environment.ProcessorCount;

        private FileStream fileStream;
        BinaryReader binaryReader;
        
        public Signature(string filePath, int lenghtBlocks)
        {
            this.filePath = filePath;
            this.lenghtBlocks = lenghtBlocks;
            FileInfo fileInfo = new FileInfo(filePath);
            this.countBlocksInFile = CalculateCountOfBlocksInFile(fileInfo.Length);

            PerformanceCounter _ramCounter = new PerformanceCounter("Memory", "Available Bytes");
            Int64 AvailableRam = (Int64)_ramCounter.NextValue();
            if ( (AvailableRam / lenghtBlocks) < countThreads)
                countThreads = (int) (AvailableRam / lenghtBlocks);
        }

        public void StartCalculateSignature()
        {
            System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
            myStopwatch.Start();

            fileStream = File.OpenRead(filePath);        
            binaryReader = new BinaryReader(fileStream);

            Thread[] threadPool = new Thread[countThreads];

            for (int i = 0; i < countThreads; i++)
            {
                threadPool[i] = new Thread(CalculateSHA256Async);
                threadPool[i].Start();
            }

            foreach (Thread item in threadPool)
            {
                item.Join();
            }

            myStopwatch.Stop();
            Console.WriteLine("Работа завершена, время выполнения: {0}", myStopwatch.Elapsed);
        }

        private void CalculateSHA256Async()
        {
            HashingBlock hashingBlock;
            while (true)
            {
                lock (binaryReader)
                    if (countDoneBlocks < countBlocksInFile)
                        hashingBlock = new HashingBlock(binaryReader.ReadBytes(lenghtBlocks), ++countDoneBlocks);
                    else
                        break;

                using (SHA256 hash = SHA256Managed.Create())
                {
                    byte[] hashValue = hash.ComputeHash(hashingBlock.contentForHash);

                    hashingBlock.contentForHash = null;

                    /* В ходе интеграционных тестов выяснилось, 
                     * что при очень большом значении lenghtBlocks GC не успевает сработать автоматически 
                     * и программа выкидывает outOfMemoryException
                     */
                    GC.Collect();

                    string hashString = "";
                    foreach (var item in hashValue)
                    {
                        hashString += String.Format("{0:X2}", item);
                    }
                    Console.WriteLine("[{0}] из [{1}] " + hashString, hashingBlock.curentBlockNumber, countBlocksInFile);
                };
            }
        }

        private long CalculateCountOfBlocksInFile(long lengthFile)
        {
            if (lengthFile % lenghtBlocks == 0)
                return lengthFile / lenghtBlocks;
            else
                return lengthFile / lenghtBlocks + 1;
        }

        #region destruct
        public void Dispose()
        {
            fileStream.Close();
            binaryReader.Close();
            GC.SuppressFinalize(this);
        }

        ~Signature()
        {
            fileStream.Close();
            binaryReader.Close();
        }
        #endregion

        private struct HashingBlock
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
