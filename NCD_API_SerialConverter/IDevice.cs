using System.Collections.Generic;

namespace NCD_API_SerialConverter
{
    public interface IDevice
    {
        IEnumerable<byte> CommandData();
    }
}
