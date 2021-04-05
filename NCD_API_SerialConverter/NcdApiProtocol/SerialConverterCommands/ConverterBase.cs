namespace NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands
{
    using System.Collections.Generic;
    using BoosterPumpLibrary.Commands;
    using NcdApiProtocol;

    public abstract class ConverterBase : DeviceBase<CommandBase>
    {
        public override byte Length => (byte)(Payload.Length + 1);

        public override byte Command => 0xFE;

        public abstract byte[] Payload { get; }

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
