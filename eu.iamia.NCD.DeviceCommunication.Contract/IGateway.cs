namespace eu.iamia.NCD.DeviceCommunication.Contract
{
    public interface IGateway
    {
        public INcdApiProtocol Execute(INcdApiProtocol i2CCommand);
    }
}