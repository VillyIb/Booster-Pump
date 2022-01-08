using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace eu.iamia.NCD.Shared.UnitTest
{
    public class NcdApiProtocolShould
    {
        private static INcdApiProtocol Sut => new NcdApiProtocol(new byte[] {0x55});

        [Fact]
        public void BeAssignableToINcdApiProtocol()
        {
            // ReSharper disable once RedundantCast
            Assert.NotNull(Sut as INcdApiProtocol);
        }

        [Fact]
        public void ReturnRightValueForCalculateChecksum()
        {
            var sut = new NcdApiProtocol(new byte[] { 0x55, 0x44, 0x33 });
            var checksum = sut.CalculatedChecksum;
            Assert.Equal(121,checksum);
        }

        [Fact]
        public void ReturnRightPropertyValues()
        {
            Assert.Equal(0xAA, Sut.Header);
            Assert.Equal(0x01, Sut.ByteCount);
            Assert.Equal(new byte[] {0x55}, Sut.Payload);
            Assert.Equal(0x00, Sut.Checksum);
        }

        [Fact]
        public void BeValid()
        {
            Assert.True(Sut.IsValid);
        }

        [Fact]
        public void NotBeValidForWrongByteCount()
        {
            var sut = new NcdApiProtocol(0xAA, 0x02, new byte[] { 0x55 }, 0x00);
            Assert.False(sut.IsValid);
        }

        [Fact]
        public void NotBeValidForWrongChecksum()
        {
            var sut = new NcdApiProtocol(0xAA, 0x01, new byte[] { 0x55 }, 0x01);
            Assert.False(sut.IsValid);
        }

        [Fact]
        public void ThrowExceptionForNullPayload()
        {
            Assert.Throws<ArgumentNullException>(() => new NcdApiProtocol(0xAA, 0xff, null, 0x00));
            Assert.Throws<ArgumentNullException>(() => new NcdApiProtocol(null));
        }

        [Fact]
        public void ThrowExceptionForTooLargePayload()
        {
            var tooLargePayload = Enumerable.Repeat<byte>(0x01, 256).ToList();
            Assert.Throws<ArgumentException>(() => new NcdApiProtocol(0xAA, 0xff, tooLargePayload, 0x00));
            Assert.Throws<ArgumentException>(() => new NcdApiProtocol(tooLargePayload));
        }

        [Fact]
        public void ReturnRightByteSequenceForGetApiEncodedData()
        {
            var expectedByteSequence = new List<byte> {0xAA, 0x01, 0x55, 0x00};
            Assert.Equal(expectedByteSequence, Sut.GetApiEncodedData());
        }

        [Fact]
        public void ReturnRightStringForPayloadAsHex()
        {
            Assert.Equal("55 ", ((NcdApiProtocol) Sut).PayloadAsHex);
        }

        [Fact]
        public void ReturnRightStringForToString()
        {
            Assert.Equal("AA 01 55 00 ", Sut.ToString());
        }

        [Theory]
        [InlineData(new byte[] { 0xBC, 0x5A, 0xA5, 0x43 })]
        [InlineData(new byte[] { 0xBC, 0x5B, 0xA5, 0x43 })]
        [InlineData(new byte[] { 0xBC, 0x5C, 0xA5, 0x43 })]
        public void ReturnErrorForSpecificPayload(byte[] payload)
        {
            var sut = new NcdApiProtocol(payload);
            Assert.True(sut.IsError);
        }
    }
}