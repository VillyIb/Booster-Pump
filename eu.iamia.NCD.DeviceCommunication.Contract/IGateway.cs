using System;

namespace eu.iamia.NCD.Serial.Contract
{
    public interface IGateway : IDisposable
    {
        public INcdApiProtocol Execute(INcdApiProtocol i2CCommand);
    }
}