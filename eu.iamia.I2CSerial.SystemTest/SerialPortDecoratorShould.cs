using System;
using System.Collections.Generic;
using System.Threading;
using BoosterPumpConfiguration;
using eu.iamia.Configuration;
using Xunit;

namespace eu.iamia.ReliableSerialPort.SystemTest
{
    /// <summary>
    /// NOTE System tests requires attached hardware.
    /// </summary>
    public class SerialPortDecoratorShould
    {
        private ISerialPortSettings SerialPortSettings;

        private void Init()
        {
            var configuration = ConfigurationSetup.Init();
            SerialPortSettings = configuration.Parse();
        }

        [Fact]
        public void OpenConnectedPort()
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

        [Fact]
        public void ReceiveSpecificResponseOnCommand_Test2WayCommunication()
        {
            var received = new List<byte>();

            Init();
            using var sut =  new SerialPortDecorator(SerialPortSettings);
            sut.Open();
            sut.DataReceived += (sender, args) =>
            {
                foreach (var current in args.Data)
                {
                    received.Add(current);
                }
            };

            sut.Write(new List<byte>{0xAA, 0x02, 0xFE, 0x21, 0xCB});
            Thread.Sleep(110);

            Assert.Equal(new List<byte>{0xAA, 0x01, 0x55, 0x00}, received);
        }

        /// <summary>
        /// Required devices: 0x48, 0x50, 0x58
        /// </summary>
        [Theory]
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
        public void ReceiveSpecificResponseOnCommand_BusScanCommand(int delay)
        {
            var received = new List<byte>();

            Init();
            using var sut = new SerialPortDecorator(SerialPortSettings);
            sut.Open();
            sut.DataReceived += (sender, args) =>
            {
                foreach (var current in args.Data)
                {
                    received.Add(current);
                }
            };

            sut.Write(new List<byte> { 0xAA, 0x02, 0xC1, 0x00, 0x6D });
            Thread.Sleep(delay);

            Assert.Equal(new List<byte> { 0xAA, 0x03, 0x48, 0x50, 0x58, 0x9d }, received);
        }

    }
}
