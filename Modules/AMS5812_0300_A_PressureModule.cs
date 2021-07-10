using eu.iamia.NCD.API.Contract;

namespace Modules
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    public class AMS5812_0300_A_PressureModule : AMS5812_0150_D_B_Module
    {
        public override float OutputPressureMax => 2068f;
        public override float OutputPressureMin => 0f;

        public AMS5812_0300_A_PressureModule(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        { }
    }
}
