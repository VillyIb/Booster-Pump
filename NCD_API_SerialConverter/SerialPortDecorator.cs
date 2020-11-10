using NCD_API_SerialConverter.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using NCD_API_SerialConverter.NcdApiProtocol;

namespace NCD_API_SerialConverter
{
    [ExcludeFromCodeCoverage]
    public class SerialPortDecorator : INCD_API_SerialPort, IDisposable
    {
        protected SerialPort SerialPort { get; private set; }

        protected ReadNcdApiFormat ReadUtil { get; private set; }

        private readonly string PortName;
        private readonly int BaudRate;
        private readonly int Timeout;

        public SerialPortDecorator(string portName)
        {
            PortName = portName;
            BaudRate = 115200;
            Timeout = 1000; // ms
            ReadUtil = new ReadNcdApiFormat(ReadByte, ReadBlock);
        }

        public void Open()
        {
            if (null != SerialPort && SerialPort.IsOpen) { SerialPort.Close(); }
            SerialPort = new SerialPort(PortName, BaudRate);
            SerialPort.Open();
            SerialPort.ReadTimeout = Timeout;
        }

        public void Close()
        {
            if (null == SerialPort) { return; }
            try
            {
                SerialPort.Close();
            }
            catch (IOException)
            { }

            SerialPort = null;
        }

        /// <summary>
        /// Discard all data in iput buffer.
        /// </summary>
        public void DiscardInBuffer()
        {
            SerialPort.DiscardInBuffer();
        }

        public int ReadTimeout
        {
            set => SerialPort.ReadTimeout = value;
            get => SerialPort.ReadTimeout;
        }

        public void Write(IEnumerable<byte> byteSequence)
        {
            var dataArray = byteSequence.ToArray();
            SerialPort.DiscardInBuffer();
            SerialPort.Write(dataArray, 0, dataArray.Length);
        }

        protected int ReadByte()
        {
            return SerialPort.ReadByte();
        }

        protected IEnumerable<byte> ReadBlock()
        {
            int bytesToRead = SerialPort.BytesToRead;
            var buffer = new byte[bytesToRead];
            var actualRead = SerialPort.Read(buffer, 0, bytesToRead);
            return buffer.Take(actualRead);
        }

        /// <summary>
        /// Reads input into NCD API specific format.
        /// </summary>
        /// <returns></returns>
        public DataFromDevice Read()
        {
            return ReadUtil.Read();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects).
                try
                {
                    SerialPort?.Dispose();
                }
                catch
                { }
            }
            // release unmanaged resources
            SerialPort = null;
        }

        ~SerialPortDecorator()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
