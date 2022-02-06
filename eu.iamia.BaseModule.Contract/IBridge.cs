using System;

namespace eu.iamia.BaseModule.Contract
{
    public interface IBridge : IDisposable
    {
        public INcdApiProtocol Execute(ICommand command);
    }
}