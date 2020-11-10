namespace NCD_API_SerialConverter.Commands
{
    using System.Linq;
    using System.Collections.Generic;
    using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;

    public class NCD_API_Converter_Scan_Command : ConverterCommandBase
    {
        public override byte Length => (byte)(Payload.Count() + 1);

        public override byte Command => CommandCodes.Scan;
              
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
