namespace NCD_API_SerialConverter.Commands
{
    using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;

    public class NCD_API_Converter_Stop_Command : ConverterCommandBase
    {
        public override byte[] Payload => new byte[] { 0x21, 0xBB };
    }
}
