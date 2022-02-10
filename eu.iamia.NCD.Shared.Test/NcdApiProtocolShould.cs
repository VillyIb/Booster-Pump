using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using eu.iamia.NCD.API.Contract;

namespace eu.iamia.NCD.Shared.UnitTest
{
    public class NcdApiProtocolShould
    {
        private static NcdApiProtocol Sut => NcdApiProtocol.WriteSuccess;

        [Fact]
        public void BeAssignableToINcdApiProtocol()
        {
            // ReSharper disable once RedundantCast
            Assert.NotNull(Sut as INcdApiProtocol);
        }

        [Fact]
        public void ReturnRightValueForCalculateChecksum()
        {       
            var checksum = Sut.CalculatedChecksum;
            Assert.Equal(0, checksum);
        }

        [Fact]
        public void ReturnRightPropertyValues()
        {
            Assert.Equal(0xAA, Sut.Header);
            Assert.Equal(0x01, Sut.ByteCount);
            Assert.Equal(new byte[] { 0x55 }, Sut.Payload);
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
            var expectedByteSequence = new List<byte> { 0xAA, 0x01, 0x55, 0x00 };
            Assert.Equal(expectedByteSequence, Sut.GetApiEncodedData());
        }

        [Fact]
        public void ReturnRightStringForPayloadAsHex()
        {
            Assert.Equal("55 ", ((NcdApiProtocol)Sut).PayloadAsHex);
        }

        [Fact]
        public void ReturnRightStringForToString()
        {
            Assert.Equal("AA 01 55 00 ", Sut.ToString());
        }

        [Fact]
        public void ReturnErrorForNoResponse()
        {
            Assert.True(NcdApiProtocol.NoResponse.IsError);
        }

        [Fact]
        public void ReturnErrorForInvalidAddress()
        {
            Assert.True(NcdApiProtocol.InvalidAddress.IsError);
        }

        [Fact]
        public void ReturnErrorForTimeout()
        {
            Assert.True(NcdApiProtocol.Timeout.IsError);
        }

        [Fact]
        public void BeEqualForIdenticalConent()
        {
            var expected = new NcdApiProtocol(new byte[] { 0x55, 0x56 });
            var actual = new NcdApiProtocol(new byte[] { 0x55, 0x56 });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BeDifferentForDifferentContent()
        {
            var expected = new NcdApiProtocol(new byte[] { 0x55, 0x56 });
            var actual = new NcdApiProtocol(new byte[] { 0x55, 0x57 });

            Assert.NotEqual(expected, actual);
        }

        [Fact]
        public void BeDifferentForNull()
        {
            var expected = new NcdApiProtocol(new byte[] { 0x55, 0x56 });

            Assert.False(expected.Equals(null));
            Assert.False(expected.Equals((object)null));
        }       
    }
}