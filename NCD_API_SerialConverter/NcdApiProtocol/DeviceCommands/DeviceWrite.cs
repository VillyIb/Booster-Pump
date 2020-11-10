namespace NCD_API_SerialConverter.NcdApiProtocol.DeviceCommands
{
    using System.Collections.Generic;
    using System.Linq;
    using BoosterPumpLibrary.Commands;
    using NcdApiProtocol;

    public class DeviceWrite : CommandBase<WriteCommand>
    {
        public DeviceWrite(WriteCommand backingValue) : base(backingValue)
        { }

        public override byte Length => (byte)(Payload.Length + 2);

        public override byte Command => 0xBE;

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
