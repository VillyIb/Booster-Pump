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
        public void ToString_WhenOk_ReturnsOk()
        {
            Assert.Equal("AA 01 55 00 ", Sut.ToString());
        }
    }
}
