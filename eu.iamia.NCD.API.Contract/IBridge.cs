using System;

namespace eu.iamia.NCD.API.Contract
{
    public interface IBridge : IDisposable
    {
        public IDataFromDevice Execute(ICommand command);
    }
}