using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eu.iamia.NCD.API.Contract;
using Xunit;

namespace eu.iamia.NCD.API.Test
{
    public class DataFromDeviceShould
    {
        private static byte[] DefaultPayload => new byte[] {0x55};

        private DataFromDevice Sut => new DataFromDevice(DefaultPayload);

        [Fact]
        public void Class_ShouldImplementIDataFromDevice()
        {
            // ReSharper disable once RedundantCast
            Assert.NotNull(Sut as IDataFromDevice);
        }

        [Fact]
        public void PayloadAsHex_WhenOk_ReturnsOk()
        {
            Assert.Equal("55 ", Sut.PayloadAsHex);
        }

        [Fact]
        public void GetApiEncodedData_WhenOk_ReturnsOk()
        {
            Assert.Equal(new byte[]{0xAA, 0x01, 0x55, 0x00}, Sut.GetApiEncodedData());
        }

        [Fact]
        public void ToString_WhenOk_ReturnsOk()
        {
            Assert.Equal("AA 01 55 00 ", Sut.ToString());
        }

        [Fact]
        public void Ctor_WhenNullPayload_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new DataFromDevice(0xAA, 0xff, null, 0x00));
            Assert.Throws<ArgumentNullException>(() => new DataFromDevice(null));
        }

        [Fact]
        public void Ctor_WhenTooLargePayload_ShouldThrowException()
        {
            var tooLargePayload = Enumerable.Repeat<byte>(0x01, 256).ToList();
            Assert.Throws<ArgumentException>(() => new DataFromDevice(0xAA, 0xff, tooLargePayload, 0x00));
            Assert.Throws<ArgumentException>(() => new DataFromDevice(tooLargePayload));
        }

        [Fact]
        public void Ctor_WhenLegalPayload_ShouldNotThrowException()
        {
            var largePayload = Enumerable.Repeat<byte>(0x01, 255).ToList();
            // ReSharper disable ObjectCreationAsStatement
            new DataFromDevice(0xAA, 0xff, largePayload, 0x00);
            new DataFromDevice(largePayload);
            // ReSharper restore ObjectCreationAsStatement
        }

    }
}
