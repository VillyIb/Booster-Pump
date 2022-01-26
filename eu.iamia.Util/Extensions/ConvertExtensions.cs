namespace eu.iamia.Util.Extensions
{
    public static class ConvertExtensions
    {
        public static sbyte ToInt8(this byte hex)
        {
            return unchecked((sbyte)hex);
        }

        public static byte ToUInt8(this sbyte hex)
        {
            return unchecked((byte)hex);
        }


        public static short ToInt16(this ushort hex) 
        {
            return unchecked((short)hex);
        }

        public static ushort ToUInt16(this short hex)
        {
            return unchecked((ushort)hex);
        }


        private const int Int24MaxValue = 0x7F_FFFF;

        private const uint UInt24MaxValue = 0x0100_0000;

        public static int ToInt24(this uint hex)
        {
            return hex > Int24MaxValue
                ? (int)(hex - UInt24MaxValue)
                : (int)hex;
        }

        public static uint ToUint24(this int value)
        {
            return value < 0
                ? (uint)(value + UInt24MaxValue)
                : (uint)value;
        }
    }
}
