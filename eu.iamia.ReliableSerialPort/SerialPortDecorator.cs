using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using eu.iamia.SerialPortSetting.Contract;
using Microsoft.Extensions.Options;
// ReSharper disable UnusedMember.Global

namespace eu.iamia.ReliableSerialPort
{
    // Requires real hardware to test.
    public class SerialPortDecorator : ISerialPortDecorator
    {
        private ISerialPortSettings SerialPortSettings { get; }

        [ExcludeFromCodeCoverage]
        // ReSharper disable once UnusedMember.Global
        public SerialPortDecorator(IOptions<ISerialPortSettings> settings)
        {
            SerialPortSettings = settings.Value;
        }

        /// <summary>
        /// Only referenced from Unit Tests
        /// </summary>
        /// <param name="settings"></param>
        internal SerialPortDecorator(ISerialPortSettings settings)
        {
            SerialPortSettings = settings;
        }

        private SerialPort SerialPort { get; set; }

        /// <summary>
        /// Subscribe to this EventHandler for data read.
        /// </summary>
        public event EventHandler<IDataReceivedArgs> DataReceived;

        private void OnDataReceived(byte[] data)
        {
            if (DataReceived is null)
            {
                throw new InvalidOperationException("No receivers");
            }

            DataReceived?.Invoke(this, new DataReceivedArgs() { Data = data });
        }

        private void ReadContinuously()
        {
            const int bufferSize = 128;
            var reusedBuffer = new byte[bufferSize];

            void ReadUntilClosed() =>
                SerialPort.BaseStream.BeginRead(
                    reusedBuffer,
                    0,
                    bufferSize,
                    delegate (IAsyncResult ar)
                    {
                        try
                        {
                            var count = SerialPort.BaseStream.EndRead(ar); // InvalidOperationException if port is closed.
                            var dst = new byte[count];
                            Buffer.BlockCopy(reusedBuffer, 0, dst, 0, count);
                            OnDataReceived(dst);
                            //Thread.Sleep(50);
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
            
            // Required to read input.
            Thread.Sleep(100);
        }

        public void Dispose()
        {
            Close();
        }
    }
}
