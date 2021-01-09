using System.Collections.ObjectModel;

namespace eu.iamia.I2CContract
{
    public interface IDataToDevice
    {
        ReadOnlyCollection<byte> Payload { get; }
    }
}
