using System;

namespace BoosterPumpLibrary.ModuleBase
{
   public class ByteWrapper
    {
        private readonly byte Payload;

        public ByteWrapper(int value)
        {
            Payload = (byte)value;
        }

        public static ByteWrapper operator +(ByteWrapper first, ByteWrapper second)
        {
            return new ByteWrapper(first.Payload + second.Payload);
        }

        public static ByteWrapper operator +(ByteWrapper first, byte second)
        {
            return new ByteWrapper(first.Payload + second);
        }

        public static implicit operator byte(ByteWrapper value)
        {
            return value.Payload;
        }

        public static implicit operator ByteWrapper(int value)
        {
            return new ByteWrapper(value);
        }
    }
}