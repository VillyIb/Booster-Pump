using System;
using BoosterPumpLibrary.Settings;
using BoosterPumpLibrary.Util;

namespace BoosterPumpLibrary.ModuleBase
{
    public interface IOutputModule
    {
        /// <summary>
        /// Default value: 1.
        /// </summary>
        public int RetryCount { get; set; }

        Guid Id { get; }

        byte DefaultAddress { get; }

        ByteWrapper AddressIncrement { get; }

        byte DeviceAddress { get; }

        public void Send(IOutputModuleEnumerator enumerator);

        public void Send();

        public void SendSpecificRegister(Register register);

        public void SetOutputRegistersDirty();

        public void SetAddressIncrement(int value);
    }
}