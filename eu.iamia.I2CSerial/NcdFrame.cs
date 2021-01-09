using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace eu.iamia.I2CSerial
{
    public class NcdFrame
    {
        public byte Header => 0xAA;

        ReadOnlyCollection<byte> Payload { get; }

        public byte ByteCount => (byte)Payload.Count;

        public byte Checksum => (byte)(Payload.Aggregate(Header + ByteCount, (result, current) => result + current) & 0xff);

        public NcdFrame(IEnumerable<byte> payload)
        {
            Payload = payload.ToList().AsReadOnly();
        }

        public IEnumerable<byte> BytesToTransmit()
        {
             yield return Header;
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
