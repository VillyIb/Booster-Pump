using System.Collections.Generic;

namespace NCD_API_SerialConverter.Contracts
{
    /// <summary>
    /// Low level contract for sending to- and receiving data from a NCD device.
    /// </summary>
    public interface INCD_API_SerialPort
    {
        /// <summary>
        /// Sends the specified byteSequence to a NCD device.
        /// </summary>
        /// <param name="byteSequence"></param>
        void Write(IEnumerable<byte> byteSequence);

        IEnumerable<byte> Read();
    }
}
