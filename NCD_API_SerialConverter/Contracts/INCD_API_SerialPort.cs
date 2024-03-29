﻿using System.Collections.Generic;
using NCD_API_SerialConverter.NcdApiProtocol;

namespace NCD_API_SerialConverter.Contracts
{
    /// <summary>
    /// Low level contract for sending to- and receiving data from a NCD device.
    /// </summary>
    public interface INcdApiSerialPort
    {
        void Open();

        /// <summary>
        /// Sends the specified byteSequence to a NCD device.
        /// </summary>
        /// <param name="byteSequence"></param>
        void Write(IEnumerable<byte> byteSequence);

        DataFromDevice Read();
    }
}
