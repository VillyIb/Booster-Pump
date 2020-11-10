using System.Collections.Generic;

namespace NCD_API_SerialConverter.Contracts
{
    public interface IX
    {
        public IEnumerable<byte> ApiEncodedData();
    }
}
