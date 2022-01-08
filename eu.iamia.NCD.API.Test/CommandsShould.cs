using System;
using System.Linq;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.Shared;
using Xunit;

// ReSharper disable InconsistentNaming

namespace eu.iamia.NCD.API.UnitTest
{
    public class CommandsShould
    {
        protected byte DefaultAddress => 0x33;
        protected byte DefaultReadLength = 0x32;
        protected static byte[] DefaultPayload => new byte[] {0x55};
    }

    public class CommandControllerControllerBusSCanShould : CommandsShould
    {
        private static Command Sut => new CommandControllerControllerBusSCan();

        [Fact]
        public void Class_ShouldImplementICommand()
        {
            // ReSharper disable once RedundantCast
            Assert.NotNull(Sut as ICommand);
        }

        [Fact]
        public void I2CDataAsHex_WhenOk_ReturnsOk()
        {
            Assert.Equal("00 ", Sut.I2CDataAsHex);
        }

        [Fact]
        public void ReturnRightCommandCode()
        {
            var actual = Sut.GetI2CCommandCode;
            Assert.Equal(I2CCommandCode.DeviceBusScan, actual);
        }
    }

    public class CommandControllerControllerHardRebootShould : CommandsShould
    {
        private static Command Sut => new CommandControllerControllerHardReboot();

        [Fact]
        public void Class_ShouldImplementICommand()
        {
            // ReSharper disable once RedundantCast
            Assert.NotNull(Sut as ICommand);
        }

        [Fact]
        public void I2CDataAsHex_WhenOk_ReturnsOk()
        {
            Assert.Equal("21 BD ", Sut.I2CDataAsHex);
        }

        [Fact]
        public void ReturnRightCommandCode()
        {
            var actual = Sut.GetI2CCommandCode;
            Assert.Equal(I2CCommandCode.DeviceConverterCommand, actual);
        }
    }

    public class CommandControllerControllerRebootShould : CommandsShould
    {
        private static Command Sut => new CommandControllerControllerReboot();

        [Fact]
        public void Class_ShouldImplementICommand()
        {
            // ReSharper disable once RedundantCast
            Assert.NotNull(Sut as ICommand);
        }

        [Fact]
        public void I2CDataAsHex_WhenOk_ReturnsOk()
        {
            Assert.Equal("21 BC ", Sut.I2CDataAsHex);
        }
        [Fact]
        public void ReturnRightCommandCode()
        {
            var actual = Sut.GetI2CCommandCode;
            Assert.Equal(I2CCommandCode.DeviceConverterCommand, actual);
        }
    }

    public class CommandControllerControllerStopShould : CommandsShould
    {
        private static Command Sut => new CommandControllerControllerStop();

        [Fact]
        public void Class_ShouldImplementICommand()
        {
            // ReSharper disable once RedundantCast
            Assert.NotNull(Sut as ICommand);
        }

        [Fact]
        public void I2CDataAsHex_WhenOk_ReturnsOk()
        {
            Assert.Equal("21 BB ", Sut.I2CDataAsHex);
        }

        [Fact]
        public void ReturnRightCommandCode()
        {
            var actual = Sut.GetI2CCommandCode;
            Assert.Equal(I2CCommandCode.DeviceConverterCommand, actual);
        }
    }

    public class CommandControllerControllerTest2WayCommunicationShould : CommandsShould
    {
        private static Command Sut => new CommandControllerControllerTest2WayCommunication();

        [Fact]
        public void Class_ShouldImplementICommand()
        {
            // ReSharper disable once RedundantCast
            Assert.NotNull(Sut as ICommand);
        }

        [Fact]
        public void I2CDataAsHex_WhenOk_ReturnsOk()
        {
            Assert.Equal("21 ", Sut.I2CDataAsHex);
        }

        [Fact]
        public void ReturnRightCommandCode()
        {
            var actual = Sut.GetI2CCommandCode;
            Assert.Equal(I2CCommandCode.DeviceConverterCommand, actual);
        }
    }

    public class CommandReadShould : CommandsShould
    {
        private Command Sut => new CommandRead(DefaultAddress, DefaultReadLength);

        [Fact]
        public void Class_ShouldImplementICommand()
        {
            // ReSharper disable once RedundantCast
            Assert.NotNull(Sut as ICommand);
        }

        [Fact]
        public void I2CDataAsHex_WhenOk_ReturnsOk()
        {
            Assert.Equal("33 32 ", Sut.I2CDataAsHex);
        }

        [Fact]
        public void ReturnRightCommandCode()
        {
            var actual = Sut.GetI2CCommandCode;
            Assert.Equal(I2CCommandCode.DeviceRead, actual);
        }
    }

    public class CommandWriteShould : CommandsShould
    {
        private Command Sut => new CommandWrite(DefaultAddress, DefaultPayload);

        [Fact]
        public void Class_ShouldImplementICommand()
        {
            // ReSharper disable once RedundantCast
            Assert.NotNull(Sut as ICommand);
        }

        [Fact]
        public void I2CDataAsHex_WhenOk_ReturnsOk()
        {
            Assert.Equal("33 55 ", Sut.I2CDataAsHex);
        }

        [Fact]
        public void Ctor_WhenTooLargePayload_ShouldThrowException()
        {
            var tooLargePayload = Enumerable.Repeat<byte>(0x01, 256).ToList();
            Assert.Throws<ArgumentException>(() => new CommandWrite(DefaultAddress, tooLargePayload));
        }

        [Fact]
        public void Ctor_WhenLegalPayload_ShouldNotThrowException()
        {
            var largePayload = Enumerable.Repeat<byte>(0x01, 255).ToList();
            // ReSharper disable once ObjectCreationAsStatement
            new CommandWrite(DefaultAddress, largePayload);
        }

        [Fact]
        public void ReturnRightCommandCode()
        {
            var actual = Sut.GetI2CCommandCode;
            Assert.Equal(I2CCommandCode.DeviceWrite, actual);
        }
    }

    public class CommandWriteReadShould : CommandsShould
    {
        private Command Sut => new CommandWriteRead(DefaultAddress, DefaultPayload, DefaultReadLength);

        [Fact]
        public void Class_ShouldImplementICommand()
        {
            // ReSharper disable once RedundantCast
            Assert.NotNull(Sut as ICommand);
        }

        [Fact]
        public void Ctor_WhenTooLargePayload_ShouldThrowException()
        {
            var tooLargePayload = Enumerable.Repeat<byte>(0x01, 256).ToList();
            Assert.Throws<ArgumentException>(() =>
                new CommandWriteRead(DefaultAddress, tooLargePayload, DefaultReadLength));
        }

        [Fact]
        public void Ctor_WhenLegalPayload_ShouldNotThrowException()
        {
            var largePayload = Enumerable.Repeat<byte>(0x01, 255).ToList();
            // ReSharper disable once ObjectCreationAsStatement
            new CommandWriteRead(DefaultAddress, largePayload, DefaultReadLength);
        }


        [Fact]
        public void I2CDataAsHex_WhenOk_ReturnsOk()
        {
            var sut = new CommandWriteRead(0x33, DefaultPayload, DefaultReadLength);
            Assert.Equal("33 32 16 55 ", sut.I2CDataAsHex);
        }

        [Fact]
        public void ReturnRightCommandCode()
        {
            var actual = Sut.GetI2CCommandCode;
            Assert.Equal(I2CCommandCode.DeviceWriteRead, actual);
        }
    }
}