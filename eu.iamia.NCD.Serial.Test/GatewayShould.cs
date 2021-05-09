using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using eu.iamia.NCD.API;
using eu.iamia.NCD.DeviceCommunication.Contract;
using eu.iamia.ReliableSerialPort;
using NSubstitute;
using Xunit;

namespace eu.iamia.NCD.Serial.Test
{
    public class GatewayShould
    {
        private ISerialPortDecorator FakeSerialPortDecorator;
        private DeviceFactory DeviceFactory;
        private IGateway Sut;

        private void Init()
        {
            FakeSerialPortDecorator = Substitute.For<ISerialPortDecorator>();
            Sut = new SerialGateway(FakeSerialPortDecorator);
            DeviceFactory = new DeviceFactory();
        }

        [Fact]
        public void ImplementsIGateway()
        {
            Init();
            Assert.False(Sut is null);
        }

        [Fact]
        public void CallsOpenUponFirstExecute()
        {
            Init();

            var command = new CommandRead(0xf1, 1);

            var i2CCommand = DeviceFactory.GetI2CCommand(command);
            Sut.Execute(i2CCommand);
            Sut.Execute(i2CCommand);

            FakeSerialPortDecorator.Received(1).Open();
            FakeSerialPortDecorator.Received(2).Write(Arg.Any<IEnumerable<byte>>());
        }

        [Fact]
        public void CallsWriteForEachExecute()
        {
            Init();

            var command = new CommandRead(0xf1, 1);

            var i2CCommand = DeviceFactory.GetI2CCommand(command);
            Sut.Execute(i2CCommand);
            Sut.Execute(i2CCommand);

            FakeSerialPortDecorator.Received(2).Write(Arg.Any<IEnumerable<byte>>());
        }

        [Fact]
        public void CallsDisposeUponDispose()
        {
            Init();

            var command = new CommandRead(0xf1, 1);

            var i2CCommand = DeviceFactory.GetI2CCommand(command);
            Sut.Execute(i2CCommand);

            ((SerialGateway)Sut).Dispose();
            FakeSerialPortDecorator.Received(1).Dispose();
        }

        [Fact]
        public void ReceiveResponseFromExecute()
        {
            Init();

            var expectedResponse = new List<byte> { 0x55, 0x56 };
            var overflow = new List<byte> {0x99, 0xFF};
            List<byte> fakeResponse = new DataFromDevice(expectedResponse).GetApiEncodedData().ToList();
                fakeResponse.AddRange( overflow);
            var command = new CommandRead(0xf1, 1);


            FakeSerialPortDecorator fakeSerialPortDecorator = Substitute.ForPartsOf<FakeSerialPortDecorator>();
            fakeSerialPortDecorator.GetResponse().Returns(fakeResponse);
            Sut = new SerialGateway(fakeSerialPortDecorator);

            var i2CCommand = DeviceFactory.GetI2CCommand(command);
            var response = Sut.Execute(i2CCommand);

            Assert.Equal(expectedResponse, response.Payload);
            Assert.True(response.IsValid);
        }

    }

    [ExcludeFromCodeCoverage]
    public class FakeSerialPortDecorator : ISerialPortDecorator
    {
        public virtual IEnumerable<byte> GetResponse() => new List<byte> { 0x00 };

        public void Dispose()
        { }

        public event EventHandler<DataReceivedArgs> DataReceived;

        public void Close()
        { }

        public void Open()
        { }

        public void Write(IEnumerable<byte> byteSequence)
        {
            // Response is passed as DataReceived.
            DataReceived?.Invoke(this, new DataReceivedArgs { Data = GetResponse().ToArray() });
        }
    }
}
