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
        protected int Mask => (1 << Size) - 1;

        /// <summary>
        /// BitSetting start position value: 0..7
        /// value is shifted this number of bits.
        /// </summary>
        public int StartPositon { get; set; }

        /// <summary>
        /// Number of bits in setting , value: 1..8 (1..16/1..24/1....)
        /// </summary>
        public int Size { get;  set; }

        public string Description { get; protected set; }

        public bool Writeable { get; protected set; }

        public Register ParentRegister { get; set; }

        private void CheckRange(int value)
        {
            // todo verify 0 <= value <= 2**size
        }

        public int Value
        {
            get
            {
                var m2 = Mask << (Size-1);
                var v2 = ParentRegister.Value & m2;
                var v1 = v2 >> (Size-1);
                return v1;
            }

            set
            {
                var v1 = value & Mask;
                var v2 = v1 << (Size-1);
                var m2 = Mask << (Size-1);

                ParentRegister.SetDataRegister( ParentRegister.Value & ~m2 | v2);
            }
        }
    }
}
