namespace NCD_API_SerialConverter.Commands
{
    public static class CommandCodes
    {
        public const byte Converter = 0xFE;

        public const byte Read = 0xBF;
        public const byte Scan = 0xC1;
        public const byte Test2Way = 0xBF;
        public const byte Write = 0xBE;
        public const byte WriteRead = 0xC0;
    }
}
