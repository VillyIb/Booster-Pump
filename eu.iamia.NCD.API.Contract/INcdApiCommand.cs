using System.Collections.Generic;

namespace eu.iamia.NCD.API.Contract
{
    public interface INcdApiCommand
    {
        IEnumerable<byte> I2C_Data();
    }
}