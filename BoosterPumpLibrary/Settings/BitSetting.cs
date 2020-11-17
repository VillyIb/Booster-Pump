using System;
using System.Collections.Generic;
using System.Text;

namespace BoosterPumpLibrary.Settings
{
    public class BitSetting
    {
        /// <summary>
        /// 
        /// </summary>
        protected ulong Mask => (1UL << Size) - 1;

        /// <summary>
        /// BitSetting start position value: 0..N where N = 
        /// value is shifted this number of bits.
        /// </summary>
        public int Offsett { get; protected set; }

        public BitSetting(int size, int offsett)
        {
            if ( size < 0 || offsett < 0 || (size + offsett) > 32) { throw new ArgumentOutOfRangeException($"Size + offeset must be less or equal to 32."); }
            Size = size;
            Offsett = offsett;
        }

        /// <summary>
        /// Number of bits in setting , value: 1..8 (1..16/1..24/1....)
        /// </summary>
        public int Size { get;  protected set; }

        public string Description { get; protected set; }

        public bool Writeable { get; protected set; }

        public Register ParentRegister { get; set; }

        private void CheckRange(ulong value)
        {
            const int min = 0;
            ulong max = (1UL << Size)-1;

            if(value < min || max < value)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Valid range: [0..{1 << Size}]");
            }
        }

        public ulong Value
        {
            get
            {
                var m2 = Mask << Offsett;
                var v2 = ParentRegister.Value & m2;
                var v1 = v2 >> Offsett;
                return v1;
            }

            set
            {
                CheckRange(value);
                var v1 = value & Mask;
                var v2 = v1 << Offsett;
                var m2 = Mask << Offsett;

                ParentRegister.SetDataRegister( ParentRegister.Value & ~m2 | v2);
            }
        }
    }
}
