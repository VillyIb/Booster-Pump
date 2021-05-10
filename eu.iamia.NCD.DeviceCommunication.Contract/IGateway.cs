namespace eu.iamia.NCD.Serial.Contract
{
    public interface IGateway
    {
        public INcdApiProtocol Execute(INcdApiProtocol i2CCommand);
    }
}