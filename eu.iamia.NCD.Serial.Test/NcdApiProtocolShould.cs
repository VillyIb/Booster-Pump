using System.Collections.Generic;
using eu.iamia.NCD.Serial.Contract;
using Xunit;

namespace eu.iamia.NCD.Serial.Test
{
    public class NcdApiProtocolShould
    {
        private static INcdApiProtocol Sut => new NcdApiProtocol(new byte[] { 0x55 });

        [Fact]
        public void ImplementINcdApiProtocolShould()
        {
            Assert.NotNull(Sut as INcdApiProtocol);
        }

        [Fact]
        public void ReturnRightValuesForProperties()
        {
            Assert.Equal(0xAA, Sut.Header);
            Assert.Equal(0x01, Sut.ByteCount);
            Assert.Equal(new byte[]{0x55}, Sut.Payload);
            Assert.Equal(0x00, Sut.Checksum);
        }

        [Fact]
        public void ReturnTrueForIsValid()
        {
            Assert.True(Sut.IsValid);
        }

        [Fact]
        public void ReturnRightByteSequenceFromGetApiEncodedData()
        {
            var expectedByteSequence = new List<byte> { Sut.Header, Sut.ByteCount };
            expectedByteSequence.AddRange(Sut.Payload);
            expectedByteSequence.Add(Sut.Checksum);

            Assert.Equal(expectedByteSequence,Sut.GetApiEncodedData());
        }

        [Fact]
        public void ReturnRightValueForPayloadAsHex()
        {
            Assert.Equal("55 ", ((NcdApiProtocol)Sut).PayloadAsHex);
        }

        [Fact]
        public void ReturnRightValueForToString()
        {
            Assert.Equal("AA 01 55 00 ", Sut.ToString());
        }


    }
}
