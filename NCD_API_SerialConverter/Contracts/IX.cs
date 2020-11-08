using System;
using System.Collections.Generic;
using System.Text;

namespace NCD_API_SerialConverter.Contracts
{
    public interface IX
    {
        public IEnumerable<byte> ApiEncodedData();
    }
}
