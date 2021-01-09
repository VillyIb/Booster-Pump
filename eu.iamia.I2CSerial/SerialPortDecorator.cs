using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Ports;
using eu.iamia.I2CContract;
using Microsoft.Extensions.Options;

namespace eu.iamia.I2CSerial
{
    [ExcludeFromCodeCoverage]
    public sealed class SerialPortDecorator : IDisposable, ISerialPortDecorator
    {
        private ISerialPortSettings SerialPortSettings { get; }

        public SerialPortDecorator(IOptions<ISerialPortSettings> settings)
        {
            SerialPortSettings = settings.Value;
        }

        public SerialPortDecorator(ISerialPortSettings settings)
        {
            SerialPortSettings = settings;
        }

        private SerialPort SerialPort { get; set; }

        /// <summary>
        /// Subscribe to this EventHandler for data read.
        /// </summary>
        public event EventHandler<DataReceivedArgs> DataReceived;

        private void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, new DataReceivedArgs { Data = data });
        }

        private void ReadContinuously()
        {
            const int BufferSize = 128;
            var reusedBuffer = new byte[BufferSize];

            void ReadUntilClosed() =>
               SerialPort.BaseStream.BeginRead(
                   reusedBuffer,
                   0,
                   BufferSize,
                   delegate (IAsyncResult ar)
                   {
                       try
                       {
                           var count = SerialPort.BaseStream.EndRead(ar); // InvalidOperationException if port is closed.
                            var dst = new byte[count];
                           Buffer.BlockCopy(reusedBuffer, 0, dst, 0, count);
                           OnDataReceived(dst);
                           ReadUntilClosed(); // loop...
                       }
                       catch (InvalidOperationException)
                       {
                           // no action, loop is broken by closed port.
                       }
                   },
                   null
               );

            ReadUntilClosed();
        }

        public void Close()
        {
            if (SerialPort is null) { return; }
            SerialPort.Close();
            SerialPort = null;
        }

        public void Open()
        {
            SerialPort?.Close();

            SerialPort = new SerialPort(SerialPortSettings.PortName, SerialPortSettings.BaudRate)
            {
                ReadTimeout = SerialPortSettings.Timeout,
                WriteTimeout = SerialPortSettings.Timeout
            };

            ReadContinuously();
        }

        public void Dispose()
        {
            Close();
        }

        private Queue<IDataFromDevice> FreeBuffers { get; set; }


        public void WriteLine(IEnumerable<byte> byteSequence)
        {
            
        }
    }

    [ExcludeFromCodeCoverage]
    public class DataReceivedArgs : EventArgs
    {
        public byte[] Data { get; set; }
    }
}
