using System;
using eu.iamia.NCD.DeviceCommunication.Contract;

namespace eu.iamia.NCD.Direct
{
    public class DirectGateway : IDirectGateway
    {
        public IDataFromDevice Execute(IDeviceCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
