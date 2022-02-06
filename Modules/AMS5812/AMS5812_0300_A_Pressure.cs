using eu.iamia.i2c.communication.contract;

namespace Modules.AMS5812
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    public class AMS5812_0300_A_Pressure : AMS5812_0150_D_Pressure
    {
        protected override float OutputPressureMax => 2068f;
        protected override float OutputPressureMin => 0f;

        public AMS5812_0300_A_Pressure(IInputModule comModule) : base(comModule)
        { }
    }
}
