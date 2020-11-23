using NCD_API_SerialConverter.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Diagnostics.CodeAnalysis;
using NCD_API_SerialConverter.NcdApiProtocol;
// ReSharper disable UnusedMember.Global

namespace NCD_API_SerialConverter
{
    [ExcludeFromCodeCoverage]
    public class SerialPortDecorator : INcdApiSerialPort, IDisposable
    {
        protected SerialPort SerialPortSelected { get; private set; }

        protected ReadNcdApiFormat ReadUtil { get; }

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
            var ports = SerialPort.GetPortNames().ToList();
            var port = ports.Last();
            Console.WriteLine($"Selected port: {port}");
            if (null != SerialPortSelected && SerialPortSelected.IsOpen) { SerialPortSelected.Close(); }
            SerialPortSelected = new SerialPort(port, BaudRate);
            SerialPortSelected.Open();
            SerialPortSelected.ReadTimeout = Timeout;
        }

        public void Close()
        {
            if (null == SerialPortSelected) { return; }
            try
            {
                SerialPortSelected.Close();
            }
            catch (IOException)
            { }

            SerialPortSelected = null;
        }

        /// <summary>
        /// Discard all data in input buffer.
        /// </summary>
        public void DiscardInBuffer()
        {
            SerialPortSelected.DiscardInBuffer();
        }

        public int ReadTimeout
        {
            set => SerialPortSelected.ReadTimeout = value;
            get => SerialPortSelected.ReadTimeout;
        }

        public void Write(IEnumerable<byte> byteSequence)
        {
            var dataArray = byteSequence.ToArray();
            SerialPortSelected.DiscardInBuffer();
            SerialPortSelected.Write(dataArray, 0, dataArray.Length);
        }

        protected int ReadByte()
        {
            return SerialPortSelected.ReadByte();
        }

        protected IEnumerable<byte> ReadBlock()
        {
            var bytesToRead = SerialPortSelected.BytesToRead;
            var buffer = new byte[bytesToRead];
            var actualRead = SerialPortSelected.Read(buffer, 0, bytesToRead);
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
                    SerialPortSelected?.Dispose();
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                { }
            }
            // release unmanaged resources
            SerialPortSelected = null;
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
