namespace NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands
{
    using System.Linq;
    using System.Collections.Generic;
    using BoosterPumpLibrary.Commands;
    using NCD_API_SerialConverter.NcdApiProtocol;

    public abstract class ConverterBase : Command_Base<CommandBase>
    {
        public override byte Length => (byte)(Payload.Count() + 1);

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
