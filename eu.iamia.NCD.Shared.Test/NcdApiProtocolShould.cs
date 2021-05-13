using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace eu.iamia.NCD.Shared.Test
{
    public class NcdApiProtocolShould
    {
        private static INcdApiProtocol Sut => new NcdApiProtocol(new byte[] { 0x55 });

        [Fact]
        public void Class_ShouldImplementINcdApiProtocolShould()
        {
            // ReSharper disable once RedundantCast
            Assert.NotNull(Sut as INcdApiProtocol);
        }

        [Fact]
        public void Properties_WhenOk_ShouldReturnRightValues()
        {
            Assert.Equal(0xAA, Sut.Header);
            Assert.Equal(0x01, Sut.ByteCount);
            Assert.Equal(new byte[] { 0x55 }, Sut.Payload);
            Assert.Equal(0x00, Sut.Checksum);
        }

        [Fact]
        public void IsValid_WhenOk_ShouldReturnTrue()
        {
            Assert.True(Sut.IsValid);
        }

        [Fact]
        public void IsValid_WhenWrong_ShouldReturnFalse()
        {
            var sut = new NcdApiProtocol(0xAA, 0xff, new byte[] { 0x55 }, 0x00);
            Assert.False(sut.IsValid);
        }

        [Fact]
        public void Ctor_WhenNullPayload_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new NcdApiProtocol(0xAA, 0xff, null, 0x00));
            Assert.Throws<ArgumentNullException>(() => new NcdApiProtocol(null));
        }

        [Fact]
        public void Ctor_WhenTooLargePayload_ShouldThrowException()
        {
            var tooLargePayload = Enumerable.Repeat<byte>(0x01, 256).ToList();
            Assert.Throws<ArgumentException>(() => new NcdApiProtocol(0xAA, 0xff, tooLargePayload, 0x00));
            Assert.Throws<ArgumentException>(() => new NcdApiProtocol(tooLargePayload));
        }

        [Fact]
        public void Ctor_WhenLegalPayload_ShouldNotThrowException()
        {
            var largePayload = Enumerable.Repeat<byte>(0x01, 255).ToList();
            // ReSharper disable ObjectCreationAsStatement
            new NcdApiProtocol(0xAA, 0xff, largePayload, 0x00);
            new NcdApiProtocol(largePayload);
            // ReSharper restore ObjectCreationAsStatement
        }

        [Fact]
        public void GetApiEncodedData_WhenOk_ShouldReturnRightByteSequence()
        {
            var expectedByteSequence = new List<byte> { 0xAA, 0x01, 0x55, 0x00 };
            Assert.Equal(expectedByteSequence, Sut.GetApiEncodedData());
        }

        [Fact]
        public void PayloadAsHex_WhenOk_ShouldReturnRightValue()
        {
            Assert.Equal("55 ", ((NcdApiProtocol)Sut).PayloadAsHex);
        }

        [Fact]
        public void ToString_WhenOk_ShouldReturnRightValue()
        {
            Assert.Equal("AA 01 55 00 ", Sut.ToString());
        }
    }
}
