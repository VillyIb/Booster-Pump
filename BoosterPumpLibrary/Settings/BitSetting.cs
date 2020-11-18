using System;
using System.Text;

namespace BoosterPumpLibrary.Settings
{
    public class BitSetting
    {
        /// <summary>
        /// 
        /// </summary>
        protected ulong Mask => Size < 64 ? (1UL << Size) - 1 : ulong.MaxValue;

        /// <summary>
        /// BitSetting start position value: 0..N where N = 
        /// value is shifted this number of bits.
        /// </summary>
        public int Offsett { get; protected set; }

        internal BitSetting(int size, int offsett, string description = "")
        {
            Size = size;
            Offsett = offsett;
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
            const int min = 0;

            ulong max = (Size < 64) ? (1UL << Size) - 1 : ulong.MaxValue;

            if (value < min || max < value)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Valid range: [0..{max}[");
            }
        }

        public ulong Value
        {
            get
            {
                var current = ParentRegister.GetValue();
                var m2 = Mask << Offsett;
                var v2 = current & m2;
                var v1 = v2 >> Offsett;
                return v1;
            }

            set
            {
                CheckRange(value);
                var v1 = value & Mask;
                var v2 = v1 << Offsett;
                var m2 = Mask << Offsett;

                //ParentRegister.SetDataRegister( ParentRegister.Value & ~m2 | v2);
                var current = ParentRegister.GetValue();
                var next = current & ~m2 | v2;
                ParentRegister.SetValue(next);
            }
        }

        public static string ToBinary(ulong value)
        {
            var result = new StringBuilder();
            var mask = 1UL << 63;
            for (int index = 0; index < 64; index++)
            {
                result.Append((value & mask) > 0 ? "1" : "0");
                mask = mask >> 1;
                if (index % 4 == 3 && index < 63) { result.Append("_"); }
            }
            return result.ToString();
        }

        public override string ToString()
        {
            ulong m2 = (ulong)(Mask << Offsett);
            var binary = ToBinary(m2);
            return $"{Description}, Size: {Size}, Offsett: {Offsett}, Mask: {binary}";
        }
    }
}
