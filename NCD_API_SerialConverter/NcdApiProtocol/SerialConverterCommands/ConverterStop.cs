namespace NCD_API_SerialConverter.Commands
{
    using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;

    public class ConverterStop : ConverterCommandBase
    {
        public override byte[] Payload => new byte[] { 0x21, 0xBB };
    }
}
