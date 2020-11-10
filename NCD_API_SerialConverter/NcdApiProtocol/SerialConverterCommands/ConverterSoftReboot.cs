using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;

namespace NCD_API_SerialConverter.Commands
{
    public class ConverterSoftReboot : ConverterBase
    {
        public override byte[] Payload => new byte[] { 0x21, 0xBC };
    }
}
