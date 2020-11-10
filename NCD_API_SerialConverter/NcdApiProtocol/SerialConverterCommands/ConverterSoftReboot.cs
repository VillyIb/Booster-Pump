namespace NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands
{
    public class ConverterSoftReboot : ConverterBase
    {
        public override byte[] Payload => new byte[] { 0x21, 0xBC };
    }
}
