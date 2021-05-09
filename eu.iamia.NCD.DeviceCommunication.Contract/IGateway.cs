using System;

namespace eu.iamia.NCD.DeviceCommunication.Contract
{
    public interface IGateway
    {
        [Obsolete("Use: INcdApiProtocol Execute(INcdApiProtocol i2CCommand)")]
        public IDataFromDevice Execute(ICommand command);

        public INcdApiProtocol Execute(INcdApiProtocol i2CCommand);
    }
}