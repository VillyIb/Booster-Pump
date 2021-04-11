using System;
using System.Collections.Generic;
using System.Text;
using eu.iamia.NCD.DeviceCommunication.Contract;

namespace eu.iamia.NCD.Direct
{
    public class DirectGateway : IGateway
    {
        public IDataFromDevice Execute(ICommand command)
        {
            throw new NotImplementedException();
        }
    }
}
