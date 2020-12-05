using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NCD_API_SerialConverter.NcdApiProtocol;

namespace NCD_API_SerialConverter
{
    public class ReadNcdApiFormat
    {
        /// <summary>
        /// All values read from the Serial Port.
        /// </summary>
        protected List<int> RawBuffer { get; private set; }

        protected List<byte> Buffer;

        protected DataFromDevice ReadResult { get; private set; }

        private readonly Func<int> FuncReadByte;

        private readonly Func<IEnumerable<byte>> FuncReadBlock;

        [ExcludeFromCodeCoverage]
        public ReadNcdApiFormat(Func<int> readByte, Func<IEnumerable<byte>> readBlock)
        {
            FuncReadByte = readByte;
            FuncReadBlock = readBlock;
        }

        protected int RawRead()
        {
            var value = FuncReadByte();
            RawBuffer.Add(value);
            return value;
        }

        protected void ReadUntilHeaderDetected()
        {
            const byte XfNcdApiHeader = 0xAA;
            var retry = 10;

            var header = RawRead(); // has standard timeout  // not expecting timeout

            while (header != XfNcdApiHeader)
            {
                Console.Error.WriteLine($"Wrong header {header}");
                if (retry-- <= 0) { throw new ApplicationException("Header not found"); }
                header = RawRead();
            }

            ReadResult.Header = (byte)header;
        }

        protected void ReadLength()
        {
            var length = RawRead(); // not expecting timeout
            ReadResult.ByteCount = (byte)length;
        }

        protected void ReadBlock()
        {
            if (ReadResult.ByteCount <= 0) return;

            var buffer = FuncReadBlock().ToList();
            Buffer.AddRange(buffer);
            RawBuffer.AddRange(buffer.Select(t => (int)t));
        }

        protected void ReadRest()
        {
            var stopAtIndex = ReadResult.ByteCount + 1;
            while (Buffer.Count < stopAtIndex)
            {
                var current = RawRead(); // timeout if input is corrupt.
                if (-1 == current) { return; }
                Buffer.Add((byte)current);
            }
        }

        public DataFromDevice Read()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            RawBuffer = new List<int>();
            Buffer = new List<byte>();
            ReadResult = new DataFromDevice();

            try
            {
                ReadUntilHeaderDetected();
                ReadLength();
                ReadBlock();
                ReadRest();
            }
            catch (TimeoutException exception)
            {
                Console.Error.WriteLine("TimeoutException");
                return ReadResult;
            }

            ReadResult.Payload = Buffer.Take(Buffer.Count - 1).ToArray();
            ReadResult.Checksum = Buffer.Last();

            if (ReadResult.CheckConsistency)
            {
                //throw new ApplicationException("Checksum verification failed");
                ReadResult.IsValid = true;
            }

            // check for error
            if(ReadResult.IsValid && ReadResult.ByteCount == 4)
            {
                if (0xBC == ReadResult.Payload[0])
                {
                    if (0x5A == ReadResult.Payload[1] || 0x5B == ReadResult.Payload[1] || 0xFC == ReadResult.Payload[1])
                    {
                        throw new ApplicationException("Timeout Error, Chip did Not Respond");
                    }
                    else if (0x5E == ReadResult.Payload[1])
                    {
                        throw new ApplicationException("Timeout Error, Chip did Not Acknowledge");
                    }
                }
            }


            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > 20)
            {
                 Console.WriteLine($"{ReadResult.ToString()} - {stopwatch.ElapsedMilliseconds}\t");
            }

            return ReadResult;
        }

    }
}
