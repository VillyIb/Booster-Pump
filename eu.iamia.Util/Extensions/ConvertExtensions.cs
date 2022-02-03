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


        internal const int Int24MaxValue = 0x7F_FFFF;

        internal const uint UInt24Converter = 0x0100_0000;

        /// <summary>
        /// Convert from 24bit 2'c complement uint to int.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static int ToInt24(this uint hex)
        {
            return (int)
                (hex > Int24MaxValue
                    ? hex - UInt24Converter
                    : hex
                );
        }

        /// <summary>
        /// Convert from int to 24bit 2'c complement 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint ToUint24(this int value)
        {
            return (uint)
                (value < 0
                    ? value + UInt24Converter
                    : value
                );
        }
    }
}
