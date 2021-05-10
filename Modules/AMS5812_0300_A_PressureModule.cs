using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.DeviceCommunication.Contract;

namespace Modules
{
    // ReSharper disable once InconsistentNaming
    public class AMS5812_0300_A_PressureModule : AMS5812_0150_D_B_Module
    {
        public AMS5812_0300_A_PressureModule(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        {
            OutputPressureMax = 2068f;
            OutputPressureMin = 0f;
        }
    }
}
