namespace eu.iamia.NCD.DeviceCommunication.Contract
{
    public interface IGateway
    {
        public IDataFromDevice Execute(ICommand command);
    }
}