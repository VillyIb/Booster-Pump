namespace NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands
{
    public class ConverterHardReboot : ConverterBase
    {
        public override byte[] Payload => new byte[] { 0x21, 0xBD };
    }
}
