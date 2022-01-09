using BoosterPumpLibrary.ModuleBase;
using eu.iamia.NCD.API.Contract;

namespace Modules
{
    public abstract class InputModule : BaseModuleV2
    {
        protected InputModule(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        { }

        public abstract void ReadFromDevice();

        public abstract bool IsInputValid { get; }
    }
}
