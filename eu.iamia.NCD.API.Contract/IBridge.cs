using System;

namespace eu.iamia.NCD.API.Contract
{
    public interface IBridge : IDisposable
    {
        public INcdApiProtocol Execute(ICommand command);
    }
}