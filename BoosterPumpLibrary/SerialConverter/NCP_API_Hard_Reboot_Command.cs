using BoosterPumpLibrary.SerialConverter;

namespace BoosterPumpLibrary.SerialConverter
{
    public class NCP_API_Hard_Reboot_Command : NCP_API_Stop_Command
    {
        public override byte[] Payload => new byte[] { 0x21, 0xBD };
    }
}
