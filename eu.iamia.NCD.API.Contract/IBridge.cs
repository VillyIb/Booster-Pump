using eu.iamia.NCD.API.Contract;

namespace eu.iamia.NCD.DeviceCommunication.Contract
{
    public interface IBridge
    {

        public IDataFromDevice Execute(ICommand command);
    }
}