using NCD_API_SerialConverter.NcdApiProtocol;

namespace NCD_API_SerialConverter
{
    public interface IReadNcdApiFormat
    {
        DataFromDevice Read();
    }
}