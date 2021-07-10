namespace BoosterPumpLibrary.ModuleBase
{
    /// <summary>
    /// Enables operator overloading for + and = making an int assignable to a byte.
    /// </summary>
   public class ByteExtension
    {
        private readonly byte Payload;

        public ByteExtension(int value)
        {
            Payload = (byte)value;
        }

        public static ByteExtension operator +(ByteExtension first, ByteExtension second)
        {
            return new(first.Payload + second.Payload);
        }

        public static ByteExtension operator +(ByteExtension first, byte second)
        {
            return new(first.Payload + second);
        }

        public static implicit operator byte(ByteExtension value)
        {
            return value.Payload;
        }

        public static implicit operator ByteExtension(int value)
        {
            return new(value);
        }
    }
}