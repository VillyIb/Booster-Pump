namespace NCD_API_SerialConverter.Commands
{
    using System.Collections.Generic;
    using BoosterPumpLibrary.Commands;

    public class DeviceRead : NCD_API_Command_Base<ReadCommand>
    {
        public DeviceRead(ReadCommand backingValue) : base(backingValue)
        { }

        public override byte Length => 0x03;

        public override byte Command => CommandCodes.Read;

        public byte LengthRequested => BackingValue.LengthRequested;

        public override IEnumerable<byte> CommandData()
        {
            yield return Command;
            yield return Address ?? 0x00;
            yield return LengthRequested;
        }
    }
}
