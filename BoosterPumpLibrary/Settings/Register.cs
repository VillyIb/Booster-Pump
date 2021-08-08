namespace BoosterPumpLibrary.Settings
{
    public class Register : RegisterBase<ulong>
    {
        protected override ushort MaxSize => 8;

        /// <summary>
        /// Describes a register in a I2C device with one or multiple bytes (max 8).
        /// </summary>
        /// <param name="registerAddress"></param>
        /// <param name="description"></param>
        /// <param name="byteCount"></param>
        public Register(byte registerAddress, string description, ushort byteCount) : base(registerAddress, description, byteCount)
        { }              
             
        internal override void SetValue(ulong value)
        {
            Value = value;
        }

        internal override ulong GetValue()
        {
            return Value;
        }
    }
}
