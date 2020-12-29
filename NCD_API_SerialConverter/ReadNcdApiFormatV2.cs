using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NCD_API_SerialConverter.NcdApiProtocol;

namespace NCD_API_SerialConverter
{
  
    public class ReadNcdApiFormatV2 : IReadNcdApiFormat
    {
        private readonly Func<int> FuncReadByte;

        private readonly Func<IEnumerable<byte>> FuncReadBlock;

        [ExcludeFromCodeCoverage]
        public ReadNcdApiFormatV2(Func<int> readByte, Func<IEnumerable<byte>> readBlock)
        {
            FuncReadByte = readByte;
            FuncReadBlock = readBlock;
        }

        public DataFromDevice Read()
        {
            throw new NotImplementedException();
        }
    }
}
