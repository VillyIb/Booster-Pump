namespace NCD_API_SerialConverter.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using BoosterPumpLibrary.Commands;

    public class NCD_API_Converter_Test2Way_Command : NCD_API_Command_Base<CommandBase>
    {
        public override byte Length => (byte)(Payload.Count() + 1);

        public override byte Command => CommandCodes.Converter;

        public virtual byte[] Payload => new byte[] { 0x21 };

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
