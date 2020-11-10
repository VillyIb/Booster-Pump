namespace NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands
{
    public class ConverterStop : ConverterBase
    {
        public override byte[] Payload => new byte[] { 0x21, 0xBB };
    }
}
