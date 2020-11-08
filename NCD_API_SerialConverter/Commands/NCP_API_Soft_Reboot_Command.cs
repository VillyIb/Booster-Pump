namespace NCD_API_SerialConverter.Commands
{
    public class NCP_API_Soft_Reboot_Command : NCP_API_Stop_Command
    {
        public override byte[] Payload => new byte[] { 0x21, 0xBC };
    }
}
