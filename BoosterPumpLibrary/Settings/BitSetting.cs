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

        internal BitSetting(int size, int offset, RegisterBase parentRegister, string description = "")
        {
            Size = size;
            Offset = offset;
            ParentRegister = parentRegister;
            Description = description;
        }

        /// <summary>
        /// Number of bits in setting , value: 1..8 (1..16/1..24/1....)
        /// </summary>
        public int Size { get; protected set; }

        public string Description { get; protected set; }

        private RegisterBase ParentRegister { get; }

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
            var result = new StringBuilder();

            var value = Mask << Offset;
            var last = Math.Max(Size + Offset, 8);

            for (var index = last-1; index >=0 ; index--)
            {
                var mask = 01UL << index;
                result.Append((value & mask) > 0 ? "1" : "0");
                if (index % 4 == 0 ) { result.Append("_"); }
            }

            return result.ToString().Substring(0, result.Length - 1);
        }

        public override string ToString()
        {
            return $"{Description}, Size: {Size}, Offset: {Offset}, Mask: {MaskAsBinary()}, Value: {Value}-0x{Value:X4}";
        }
    }
}
