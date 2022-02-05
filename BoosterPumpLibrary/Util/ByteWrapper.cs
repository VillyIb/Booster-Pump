using System;

namespace BoosterPumpLibrary.Util
{
    /// <summary>
    /// Wraps a byte in an invariable class and enables operator overloading for + and = making an int assignable to a byte.
    /// </summary>
    public class ByteWrapper : IEquatable<ByteWrapper>
    {
        public readonly byte Payload;

        public ByteWrapper(int value)
        {
            Payload = (byte)value;
        }
        public bool Equals(ByteWrapper? other)
        {
            if (other is null) return false;

            return Payload == other.Payload;
        }

        public static ByteWrapper operator +(ByteWrapper first, ByteWrapper second)
        {
            return new(first.Payload + second.Payload);
        }

        public static ByteWrapper operator +(ByteWrapper first, byte second)
        {
            return new(first.Payload + second);
        }

        public static implicit operator byte(ByteWrapper value)
        {
            return value.Payload;
        }

        public static implicit operator ByteWrapper(int value)
        {
            return new(value);
        }
    }
}