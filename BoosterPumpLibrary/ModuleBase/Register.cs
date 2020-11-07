namespace BoosterPumpLibrary.ModuleBase
{
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

        public void SetDataRegister(byte value)
        {
            //if (value == Data) { return; }
            Value = value;
            IsDirty = true;
        }

        public void SetDataRegisterBit(byte mask, bool value)
        {
            if (value)
            {
                if ((Value & mask) > 0) { return; }
                Value = (byte)(Value | mask);
                IsDirty = true;
            }
            else
            {
                if ((Value & mask) == 0) { return; }
                Value = (byte)(Value & ~mask);
                IsDirty = true;
            }
        }

        public void SetDirty()
        {
            IsDirty = true;
        }


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
