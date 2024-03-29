using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using eu.iamia.NCD.Bridge;
using eu.iamia.ReliableSerialPort;
using NSubstitute;
using Xunit;

namespace eu.iamia.NCD.Serial.Test
{
    public class GatewayShould
    {
        private SerialGateway Sut;
        private ISerialPortDecorator FakeSerialPortDecorator;

        private void Init()
        {
            FakeSerialPortDecorator = Substitute.For<ISerialPortDecorator>();
            Sut = new SerialGateway(FakeSerialPortDecorator);
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

            var i2CCommand = new NcdApiProtocol(Array.Empty<byte>());

            Sut.Execute(i2CCommand);
            Sut.Execute(i2CCommand);

            FakeSerialPortDecorator.Received(1).Open();
            FakeSerialPortDecorator.Received(2).Write(Arg.Any<IEnumerable<byte>>());
        }

        [Fact]
        public void CallsWriteForEachExecute()
        {
            Init();

            var i2CCommand = new NcdApiProtocol(0xf1, 1, new byte[] { 0x00 }, 0);

            Sut.Execute(i2CCommand);
            Sut.Execute(i2CCommand);

            FakeSerialPortDecorator.Received(2).Write(Arg.Any<IEnumerable<byte>>());
        }

        [Fact]
        public void CallsDisposeUponDispose()
        {
            Init();

            var i2CCommand = new NcdApiProtocol(0xf1, 1, new byte[] { 0x00 }, 0);

            Sut.Execute(i2CCommand);

            Sut.Dispose();
            FakeSerialPortDecorator.Received(1).Dispose();
        }

        [Fact]
        public void ReceiveResponseFromExecute()
        {
            Init();

            var expectedResponse = new List<byte> { 0x55, 0x56 };
            var overflow = new List<byte> { 0x99, 0xFF };
            List<byte> fakeResponse = new NcdApiProtocol(expectedResponse).GetApiEncodedData().ToList();
            fakeResponse.AddRange(overflow);
            var i2CCommand = new NcdApiProtocol(new byte[] { 0x00 });


            FakeSerialPortDecorator fakeSerialPortDecorator = Substitute.ForPartsOf<FakeSerialPortDecorator>();
            fakeSerialPortDecorator.GetResponse().Returns(fakeResponse);

            var response = Sut.Execute(i2CCommand);

            Assert.Equal(expectedResponse, response.Payload);
            Assert.True(response.IsValid);
        }

        [Fact]
        public void x()
        {
            Init();

            Sut.Dispose();
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
