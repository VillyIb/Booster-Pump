namespace SerialConverter.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using BoosterPumpLibrary.Commands;

    public class NCP_API_Stop_Command : NCD_API_Packet_Command_Base<CommandBase>
    {
        public override byte Length => (byte)(Payload.Count() + 1);

        public override byte Command => 0xFE;

        public virtual byte[] Payload => new byte[] { 0x21, 0xBB };

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
