namespace BoosterPumpLibrary.Settings
{
    public class Register : RegisterBase<ulong>
    {
        protected override int MaxSize => 8;

        public Register(byte registerIndex, string description, int byteCount) : base(registerIndex, description, byteCount)
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
