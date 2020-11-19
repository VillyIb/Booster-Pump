using System;

namespace BoosterPumpLibrary.ModuleBase
{
    [Obsolete]
    public class Register
    {
        public byte Value { get; protected set; }

        public byte RegisterId { get; protected set; }

        public bool IsDirty { get; protected set; }

        public string Description { get; protected set; }

        public string DisplayFormat { get; set; }

        public Register(byte address, string description, string displayFormat, byte defaultValue = 0)
        {
            RegisterId = address;
            Description = description;
            DisplayFormat = displayFormat;
            Value = defaultValue;
        }

        public void SetDataRegister(int value)
        {
            if(value < 0 || 0xff < value) { throw new ArgumentOutOfRangeException(nameof(value), value, "Valid: {0..255}"); }
            //if (value == Data) { return; }
            Value = (byte)value;
            IsDirty = true;
        }

        public void SetDataRegisterBit(byte mask, bool value)
        {
            if (value)
            {
                //if ((Value & mask) > 0) { return; }
                Value = (byte)(Value | mask);
                IsDirty = true;
            }
            else
            {
                //if ((Value & mask) == 0) { return; }
                Value = (byte)(Value & ~mask);
                IsDirty = true;
            }
        }

        public void SetDirty()
        {
            IsDirty = true;
        }


        // ReSharper disable once UnusedMember.Global
        public bool GetDataRegisterBit(byte mask)
        {
            return (Value & mask) > 0;
        }

        public byte GetDataRegisterAndClearDirty()
        {
            IsDirty = false;
            return Value;
        }
    }
}
