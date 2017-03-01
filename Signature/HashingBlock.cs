using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signature
{
    public class HashingBlock
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

