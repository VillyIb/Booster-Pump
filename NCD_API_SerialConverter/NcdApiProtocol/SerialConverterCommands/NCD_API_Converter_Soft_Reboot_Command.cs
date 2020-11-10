using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;

namespace NCD_API_SerialConverter.Commands
{
    public class NCD_API_Converter_Soft_Reboot_Command : ConverterCommandBase
    {
        public override byte[] Payload => new byte[] { 0x21, 0xBC };
    }
}
