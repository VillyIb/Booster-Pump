using System;
using eu.iamia.BaseModule.Contract;
using eu.iamia.NCD.Shared;

namespace eu.iamia.NCD.Serial.Contract
{
    public interface IGateway : IDisposable
    {
        public INcdApiProtocol Execute(INcdApiProtocol i2CCommand);
    }
}