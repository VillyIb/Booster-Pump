using System;

namespace eu.iamia.BaseModule.Contract
{
    public interface IBridge : IDisposable
    {
        public INcdApiProtocol Execute(eu.iamia.NCD.API.Contract.ICommand command);
    }
}