// ReSharper disable UnusedMemberInSuper.Global

using System;

namespace BoosterPumpLibrary.Contracts
{
    [Obsolete("Use: namespace eu.iamia.I2CContract")]
    public interface IDataFromDevice
    {
        byte[] Payload { get; }

        bool IsValid { get; }
    }
}
