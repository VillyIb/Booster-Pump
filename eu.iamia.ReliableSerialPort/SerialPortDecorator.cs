using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Ports;
using System.Linq;
using Microsoft.Extensions.Options;

namespace eu.iamia.ReliableSerialPort
{
    // Requires real hardware to test.
    public sealed class SerialPortDecorator : ISerialPortDecorator
    {
        private ISerialPortSettings SerialPortSettings { get; }

        [ExcludeFromCodeCoverage]
        // ReSharper disable once UnusedMember.Global
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
            DataReceived?.Invoke(this, new() { Data = data });
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

            ReadUntilClosed(); // One-time Read...
        }

        public void Close()
        {
            if (SerialPort is null) { return; }
            SerialPort.Close();
            SerialPort = null;
        }

        public void Open()
        {
            var connectedPorts = SerialPort.GetPortNames();
            if (!connectedPorts.Any(t =>
                t.Equals(SerialPortSettings.PortName, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ApplicationException($"Expected port '{SerialPortSettings.PortName}' not found");
            }

            SerialPort?.Close();

            SerialPort = new(SerialPortSettings.PortName, SerialPortSettings.BaudRate)
            {
                ReadTimeout = SerialPortSettings.Timeout,
                WriteTimeout = SerialPortSettings.Timeout
            };

            SerialPort.Open();

            ReadContinuously();
        }

        public void Write(IEnumerable<byte> byteSequence)
        {
            var output = byteSequence.ToArray();
            SerialPort.Write(output, 0, output.Length);
        }

        public void Dispose()
        {
            Close();
        }
    }
}
