using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eu.iamia.NCD.Serial.Contract;

namespace eu.iamia.NCD.Serial
{
    public class DataToDevice : IDataToDevice
    {
        public byte Header => 0xAA;

        public byte ByteCount => (byte)Payload.Count;

        public ReadOnlyCollection<byte> Payload { get; }
        
        public byte Checksum => (byte)(Payload.Aggregate(Header + ByteCount, (result, current) => result + current) & 0xff);

        public DataToDevice(IEnumerable<byte> payload)
        {
            Payload = payload.ToList().AsReadOnly();
        }

        /// <summary>
        /// Header, ByteCount, Payload[], Checksum.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> BytesToTransmit()
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
            return BytesToTransmit().Aggregate("NcdFrame: ", (result, current) => result + $"{current:X2} ");
        }
    }
}
