using System;
using System.Collections.Generic;

namespace eu.iamia.ReliableSerialPort
{
    public interface ISerialPortDecorator : IDisposable
    {
        /// <summary>
        /// Subscribe to this EventHandler to read data.
        /// </summary>
        event EventHandler<DataReceivedArgs> DataReceived;

        void Close();

        void Open();
        
        /// <summary>
        /// Send bytes to Serial Port.
        /// </summary>
        /// <param name="byteSequence"></param>
        void Write(IEnumerable<byte> byteSequence);
    }
}