namespace NCD_API_SerialConverter.NcdApiProtocol.DeviceCommands
{
    using System.Collections.Generic;
    using System.Linq;
    using BoosterPumpLibrary.Commands;
    using NCD_API_SerialConverter.NcdApiProtocol;

    public class DeviceWriteRead : Command_Base<WriteReadCommand>
    {
        public DeviceWriteRead(WriteReadCommand backingValue) : base(backingValue)
        { }

        public override byte Length => (byte)(Payload.Count() + 4);

        public override byte Command => 0xC0;

        public byte[] Payload => BackingValue.Payload.ToArray();

        public byte LengthRequested => BackingValue.LengthRequested;

        public byte Delay => BackingValue.Delay;

        public override IEnumerable<byte> CommandData()
        {
            yield return Command;
            yield return Address ?? 0x00;
            yield return LengthRequested;
            yield return Delay;
            foreach (var current in Payload)
            {
                yield return current;
            }
        }
    }
}
