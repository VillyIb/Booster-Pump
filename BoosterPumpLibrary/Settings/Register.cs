using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BoosterPumpLibrary.Settings
{
    public class Register
    {
        public byte Value { get; protected set; }

        public byte RegisterIndex { get; protected set;}

        public bool IsDirty { get; protected set; }

        public IEnumerable<BitSetting> BitSettings { get; }

        public string Description { get; protected set; }

        private  Register()
        { }

        public Register(byte registerIndex, string description, params BitSetting[] bitSettings)
        {
            RegisterIndex = registerIndex;
            Description = description;
            BitSettings = bitSettings.ToList();

            foreach(var bs in BitSettings)
            {
                bs.ParentRegister = this;
            }
        }

        public void SetDataRegister(int value)
        {
            if (value < 0 || 0xff < value) { throw new ArgumentOutOfRangeException(nameof(value), value, "Valid: {0..255}"); }
            //if (value == Data) { return; }
            Value = (byte)value;
            IsDirty = true;
        }
    }
}
