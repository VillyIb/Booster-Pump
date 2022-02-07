using System;
using System.Collections.Generic;

namespace eu.iamia.SerialPortSetting.Contract
{
    public interface ISerialPortDecorator : IDisposable
    {
        /// <summary>
        /// Subscribe to this EventHandler to read data.
        /// </summary>
        event EventHandler<IDataReceivedArgs> DataReceived;

        void Open();

        /// <summary>
        /// Send bytes to Serial Port.
        /// </summary>
        /// <param name="byteSequence"></param>
        void Write(IEnumerable<byte> byteSequence);
    }
}