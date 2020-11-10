namespace NCD_API_SerialConverter.Commands
{
    using System.Linq;
    using System.Collections.Generic;
    using BoosterPumpLibrary.Commands;

    public class NCD_API_Converter_Scan_Command : NCD_API_Command_Base<CommandBase>
    {
        public override byte Length => (byte)(Payload.Count() + 1);

        public override byte Command => CommandCodes.Scan;

        public byte[] Payload => new byte[] { 0x00 };

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
