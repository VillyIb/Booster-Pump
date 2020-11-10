namespace NCD_API_SerialConverter.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using BoosterPumpLibrary.Commands;

    public class DeviceWrite : NCD_API_Command_Base<WriteCommand>
    {
        public DeviceWrite(WriteCommand backingValue) : base(backingValue)
        { }

        public override byte Length => (byte)(Payload.Count() + 2);

        public override byte Command => CommandCodes.Write;

        public byte[] Payload => BackingValue.Payload.ToArray();

        public override IEnumerable<byte> CommandData()
        {
            yield return Command;
            yield return Address ?? 0x00;
            foreach (var current in Payload)
            {
                yield return current;
            }
        }
    }
}
