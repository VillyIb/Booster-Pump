#define HAS_HARDWARE

using System;
using System.Collections.Generic;
using System.Threading;
using BoosterPumpConfiguration;
using Xunit;
using eu.iamia.Configuration;

namespace eu.iamia.ReliableSerialPort.SystemTest
{
    using eu.iamia.SerialPortSetting.Contract;

    /// <summary>
    /// NOTE System tests requires attached hardware AND executed in sequence.
    /// </summary>
    public class SerialPortDecoratorShould
    {
        private ISerialPortSettings SerialPortSettings;

        private void Init()
        {
            var configuration = ConfigurationSetup.Init();
            SerialPortSettings = configuration.Parse();
        }

#if HAS_HARDWARE
        [Fact]
#else
        [Fact(Skip = "Requires hardware")]
#endif
        public void IT_OpenConnectedPort()
        {
            Init();
            using var sut = new SerialPortDecorator(SerialPortSettings);
            sut.Open();
        }

        [Fact]
        public void ThrowExceptionForUnknownPort()
        {
            Init();
            SerialPortSettings.PortName = "COM9";
            using var sut = new SerialPortDecorator(SerialPortSettings);

            Assert.Throws<ApplicationException>(() => sut.Open());
        }

#if HAS_HARDWARE
        [Fact]
#else
        [Fact(Skip = "Requires hardware")]
#endif
        public void ReceiveSpecificResponseOnCommand_Test2WayCommunication()
        {
            var expected = new byte[] { 0xAA, 0x01, 0x55, 0x00 };
            var received = new List<byte>();

            Init();
            using var sut = new SerialPortDecorator(SerialPortSettings);
            sut.Open();
            sut.DataReceived += (_, args) => { received.AddRange(args.Data); };

            var commandByteSequence = new List<byte> { 0xAA, 0x02, 0xFE, 0x21, 0xCB };

            sut.Write(commandByteSequence);
            Thread.Sleep(110);

            Assert.Equal(expected, received);
        }

        /// <summary>
        /// Required devices: 0x48, 0x50, 0x58½
        /// </summary>
#if HAS_HARDWARE
        [Theory]
#else
        [Theory(Skip = "Requires hardware")]
#endif
        // Repeated by intention.
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(110)]
        [InlineData(120)]
        [InlineData(130)]
        [InlineData(140)]
        [InlineData(150)]
        [InlineData(160)]
        [InlineData(170)]
        [InlineData(180)]
        [InlineData(190)]
        // Returns the Address of I2C Devices on a I2C Port
        // Requires 0x48: TCN75A TEMP, 0x50: ADC121C021 POTENTIOMETER
        public void ReceiveSpecificResponseOnCommand_BusScanCommand(int delay)
        {
            var received = new List<byte>();

            Init();
            using var sut = new SerialPortDecorator(SerialPortSettings);
            sut.Open();
            sut.DataReceived += (_, args) => { received.AddRange(args.Data); };

            sut.Write(new List<byte> { 0xAA, 0x02, 0xC1, 0x00, 0x6D });
            Thread.Sleep(delay);

            Assert.Equal(new() { 0xAA, 0x02, 0x48, 0x50, 0x44 }, received);
        }

    }
}
