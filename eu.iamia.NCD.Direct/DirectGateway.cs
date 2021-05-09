using System;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.DeviceCommunication.Contract;

namespace eu.iamia.NCD.Direct
{
    public class DirectGateway : IGateway
    {
        public IDataFromDevice Execute(ICommand command)
        {
            throw new NotImplementedException();
        }

        public INcdApiProtocol Execute(INcdApiProtocol i2CCommand)
        {
            throw new NotImplementedException();
        }
    }
}
