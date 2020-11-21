using System;
using System.Text;

namespace BoosterPumpLibrary.Settings
{
    public class BitSetting
    {
        /// <summary>
        /// 
        /// </summary>
        protected internal ulong Mask => Size < 64 ? (1UL << Size) - 1 : ulong.MaxValue;

        /// <summary>
        /// BitSetting start position value: 0..N where N = 
        /// value is shifted this number of bits.
        /// </summary>
        public int Offset { get; protected set; }

        internal BitSetting(int size, int offset, string description = "")
        {
            Size = size;
            Offset = offset;
            Description = description;
        }

        /// <summary>
        /// Number of bits in setting , value: 1..8 (1..16/1..24/1....)
        /// </summary>
        public int Size { get; protected set; }

        public string Description { get; protected set; }

        public bool Writeable { get; protected set; }

        public RegisterBase ParentRegister { get; set; }

        private void CheckRange(ulong value)
        {
            var max = Size < 64 ? (1UL << Size) - 1 : ulong.MaxValue;

            if (max < value)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Valid range: [0..{max}[");
            }
        }

        public ulong Value
        {
            get
            {
                var current = ParentRegister.GetValue();
                var m2 = Mask << Offset;
                var v2 = current & m2;
                var v1 = v2 >> Offset;
                return v1;
            }

            set
            {
                CheckRange(value);
                var v1 = value & Mask;
                var v2 = v1 << Offset;
                var m2 = Mask << Offset;

                //ParentRegister.SetDataRegister( ParentRegister.Value & ~m2 | v2);
                var current = ParentRegister.GetValue();
                var next = current & ~m2 | v2;
                ParentRegister.SetValue(next);
            }
        }

        public string MaskAsBinary()
        {
            var value = Mask << Offset;
            var result = new StringBuilder();
            var mask = 1UL << Size + Offset - 1;

            for (int index = 0; index < 64 && mask > 0; index++)
            {
                result.Append((value & mask) > 0 ? "1" : "0");
                mask = mask >> 1;
                if (index % 4 == 3 && mask > 0) { result.Append("_"); }
            }
            return result.ToString();
        }

        public override string ToString()
        {
            return $"{Description}, Size: {Size}, Offset: {Offset}, Mask: {MaskAsBinary()}";
        }
    }
}
