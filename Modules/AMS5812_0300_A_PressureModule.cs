using BoosterPumpLibrary.Contracts;

namespace Modules
{
    public class AMS5812_0300_A_PressureModule : AMS5812_0150_D_B_Module
    {
        public AMS5812_0300_A_PressureModule(ISerialConverter serialPort) : base(serialPort)
        {
            OutputPressureMax = 2068f;
            OutputPressureMin = 0f;
        }
    }
}
