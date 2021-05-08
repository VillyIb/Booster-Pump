using System.Collections.Generic;
using System.Text;
using eu.iamia.NCD.DeviceCommunication.Contract;

namespace eu.iamia.NCD.Serial
{
    /// <summary>
    /// Data returned from device.
    /// </summary>
    public class DataFromDevice : NcdApiProtocol, IDataFromDevice
    {

        public DataFromDevice(byte header, byte byteCount, IEnumerable<byte> payload, byte checksum)
            : base(header, byteCount, payload, checksum)
        { }

        public DataFromDevice(IEnumerable<byte> payload) : base(payload)
        { }

        public IEnumerable<byte> ApiEncodedData()
        {
            yield return Header;
            yield return ByteCount;
            if (null != Payload)
            {
                foreach (var current in Payload)
                {
                    yield return current;
                }
            }
            yield return Checksum;
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach (var current in ApiEncodedData())
            {
                result.AppendFormat($"{current:X2} ");
            }

            return result.ToString();
        }
    }
}
