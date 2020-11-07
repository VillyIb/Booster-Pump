using System.Collections.Generic;

namespace BoosterPumpLibrary.SerialConverter
{
    using Commands;

    public class NCD_API_Packet_Read_Command : NCD_API_Packet_Command_Base<ReadCommand>
    {
        public NCD_API_Packet_Read_Command(ReadCommand backingValue) : base(backingValue)
        { }

        public override byte Length => 0x03;

        public override byte Command => 0xBF;

        public byte LengthRequested => BackingValue.LengthRequested;

        public override IEnumerable<byte> CommandData()
        {
            yield return Command;
            yield return Address ?? 0x00;
            yield return LengthRequested;
        }
    }
}
