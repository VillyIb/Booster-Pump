namespace NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands
{
    using System.Linq;
    using System.Collections.Generic;

    public class ConverterScan : ConverterBase
    {
        public override byte Length => (byte)(Payload.Count() + 1);

        public override byte Command => 0xC1;

        public override byte[] Payload => new byte[] { 0x00 };

        public override IEnumerable<byte> CommandData()
        {
            yield return Command;
            foreach (var current in Payload)
            {
                yield return current;
            }
        }
    }
}
