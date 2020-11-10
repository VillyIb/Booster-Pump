namespace NCD_API_SerialConverter.Commands
{
    public class NCD_API_Converter_Soft_Reboot_Command : NCD_API_Converter_Stop_Command
    {
        public override byte[] Payload => new byte[] { 0x21, 0xBC };
    }
}
