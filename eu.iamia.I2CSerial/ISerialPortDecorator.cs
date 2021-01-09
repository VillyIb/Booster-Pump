using System;
using System.Collections.Generic;

namespace eu.iamia.I2CSerial
{
    public interface ISerialPortDecorator
    {
        /// <summary>
        /// Subscribe to this EventHandler for data read.
        /// </summary>
        event EventHandler<DataReceivedArgs> DataReceived;

        void Close();

        void Open();
        
        void Write(IEnumerable<byte> byteSequence);
    }
}