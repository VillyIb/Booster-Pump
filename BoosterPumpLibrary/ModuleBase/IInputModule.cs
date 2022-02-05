﻿using System;
using BoosterPumpLibrary.Util;

namespace BoosterPumpLibrary.ModuleBase
{
    public interface IInputModule
    {
        bool IsInputValid { get; }

        Guid Id { get; }

        byte DefaultAddress { get; }

        ByteWrapper AddressIncrement { get; }

        byte DeviceAddress { get; }

        /// <summary>
        /// Default value: 1.
        /// </summary>
        int RetryCount { get; set; }

        void ReadFromDevice();

        void SetInputRegistersDirty();

        void SetAddressIncrement(int value);
    }
}