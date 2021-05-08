namespace eu.iamia.NCD.DeviceCommunication.Contract
{
    public interface IGateway
    {
        public IDataFromDevice Execute(IDeviceCommand command);
    }

    public interface IDirectGateway : IGateway
    { }

    public interface ISerialGateway : IGateway
    {
        public IDataFromDevice Execute(IControllerCommand command);
    }
}