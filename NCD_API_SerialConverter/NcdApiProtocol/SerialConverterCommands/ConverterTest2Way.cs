namespace NCD_API_SerialConverter.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;

    public class NCD_API_Converter_Test2Way_Command : ConverterCommandBase
    {
        public override byte Length => (byte)(Payload.Count() + 1);

        public override byte Command => 0xFE;

        public override  byte[] Payload => new byte[] { 0x21 };

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
