using NCD_API_SerialConverter.Commands;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Ports;

namespace NCD_API_SerialConverter
{
    
    public class ReadNcdApiFormat
    {
        /// <summary>
        /// All values read from the Serial Port.
        /// </summary>
        protected List<int> RawBuffer { get; private set; }

        protected List<byte> Buffer;

        protected NCD_API_Packet_Read_Data ReadResult { get; private set; }

        private readonly Func<int> ReadFromSerialPort;

        [ExcludeforCodeCoverage]
        public ReadNcdApiFormat(Func<int> readFromSerialPort)
        {
            this.ReadFromSerialPort = readFromSerialPort;
        }

        protected SerialPort SerialPort { get; set; }

        protected int RawRead()
        {
            var value = ReadFromSerialPort();
            RawBuffer.Add(value);
            return value;
        }

        protected void ReadUntilHeaderDetected()
        {
            const byte NCD_API_Header = 0xAA;
            var retry = 10;

            var header = RawRead(); // has standard timeout  // not expecting timeout

            while (header != NCD_API_Header)
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
            if (ReadResult.ByteCount > 0)
            {
                int bytesToRead = SerialPort.BytesToRead;
                var buffer = new byte[bytesToRead];
                var actualRead = SerialPort.Read(buffer, 0, bytesToRead);
                Buffer.AddRange(buffer.Take(actualRead));
                RawBuffer.AddRange(buffer.Take(actualRead).Select(t => (int)t));
            }
        }

        protected void ReadRest()
        {
            var stopAtIndex = ReadResult.ByteCount + 3;
            while (Buffer.Count < stopAtIndex)
            {
                var current = RawRead(); // timeout if input is corrupt.
                if (-1 == current) { return; }
                Buffer.Add((byte)current);
            }
        }

        public NCD_API_Packet_Read_Data Read()
        {
            RawBuffer = new List<int>();
            Buffer = new List<byte>();
            ReadResult = new NCD_API_Packet_Read_Data();

            if (null == SerialPort || !SerialPort.IsOpen)
            {
                throw new InvalidOperationException();
            }

            try
            {
                ReadUntilHeaderDetected();
                ReadLength();
                ReadBlock();
                ReadRest();
            }
            catch (TimeoutException ex)
            {
                var msg2 = ex.Message;
                Console.Error.WriteLine("TimeoutException");
            }
            finally
            {
                SerialPort = null;
            }

            ReadResult.Payload = Buffer.Take(Buffer.Count - 1).ToArray();
            ReadResult.Checksum = Buffer.Last();

            if(!ReadResult.VerifyChecksum)
            {
                throw new ApplicationException("");// TODO
            }

            return ReadResult;
        }

    }
}
