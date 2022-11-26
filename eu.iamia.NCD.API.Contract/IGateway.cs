using System;

namespace eu.iamia.NCD.API.Contract
{
    public interface IGateway : IDisposable
    {
        public INcdApiProtocol Execute(INcdApiProtocol i2CCommand, int requestedLength);
    }
}