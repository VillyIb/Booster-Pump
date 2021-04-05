// ReSharper disable UnusedMemberInSuper.Global

using System;

namespace BoosterPumpLibrary.Contracts
{
    [Obsolete("Use: namespace eu.iamia.NCD.Serial.Contract")]
    public interface IDataFromDevice
    {
        byte[] Payload { get; }

        bool IsValid { get; }
    }
}
