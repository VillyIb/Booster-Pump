namespace SerialConverter.Commands
{
    using System.Linq;

    using System.Collections.Generic;
    using BoosterPumpLibrary.Commands;

    public class NCP_API_Scan_Command : NCD_API_Packet_Command_Base<CommandBase>
    {
        public override byte Length => (byte)(Payload.Count() + 1);

        public override byte Command => 0xC1;

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
