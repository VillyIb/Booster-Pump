using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.Serial;
using eu.iamia.NCD.Shared;
using eu.iamia.ReliableSerialPort;
using NSubstitute;
using Xunit;

namespace eu.iamia.NCD.Bridge.UnitTest
{
    public class ApiToSerialBridgeShouldFor
    {
        private ISerialPortDecorator FakeSerialPortDecorator;
        private ApiToSerialBridge Sut;

        private void Init()
        {
            FakeSerialPortDecorator = Substitute.For<ISerialPortDecorator>();
            Sut = new ApiToSerialBridge(new SerialGateway(FakeSerialPortDecorator));
        }

        [Fact]
        public void Class_ImplementIGateway()
        {
            Init();
            // ReSharper disable once RedundantCast
            Assert.NotNull(Sut as IBridge);
        }

        [Fact]
        public void Ctor_WhenCalledWithNull_ThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new ApiToSerialBridge(null));
        }

        private static readonly IList<byte> DefaultPayload = new List<byte> {0x55};
        private const byte DefaultDeviceAddress = 0x01;
        private const byte DefaultReadLength = 0x07;

        public static IEnumerable<object[]> TestData =>
            new List<object[]>
            {
                new object[] {new CommandWrite(DefaultDeviceAddress, DefaultPayload), 0xBE,},
                new object[] {new CommandRead(DefaultDeviceAddress, DefaultReadLength), 0xBF},
                new object[] {new CommandWriteRead(DefaultDeviceAddress, DefaultPayload, DefaultReadLength), 0xC0},
                new object[] {new CommandControllerControllerBusSCan(), 0xC1},
                new object[] {new CommandControllerControllerHardReboot(), 0xFE},
                new object[] {new CommandControllerControllerReboot(), 0xFE},
                new object[] {new CommandControllerControllerStop(), 0xFE},
                new object[] {new CommandControllerControllerTest2WayCommunication(), 0xFE},
            };

        [Theory]
        [MemberData(nameof(TestData))]
        public void GetI2CommandCode_WhenCalled_ReturnValidCode(ICommand command, byte expectedI2CommandCode)
        {
            Init();
            var actual = command.GetI2CCommandCode;
            Assert.Equal((I2CCommandCode) expectedI2CommandCode, actual);
        }

        [ExcludeFromCodeCoverage]
        private class NotValidCommand : ICommand
        {
            public IEnumerable<byte> I2C_Data()
            {
                throw new NotImplementedException();
            }

            public I2CCommandCode GetI2CCommandCode => throw new NotImplementedException();
        }

        [Fact]
        public void GetI2CommandCode_WhenCalledWithNotValidCommand_ThrowException()
        {
            Init();
            Assert.Throws<NotImplementedException>(() => new NotValidCommand().GetI2CCommandCode);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void GetI2Command_WhenCalled_ReturnValidCode(ICommand command, byte expectedI2CommandCode)
        {
            Init();
            var actual = Sut.GetI2CCommand(command);
            var actualCommandCode = actual.Payload.First();
            Assert.Equal(expectedI2CommandCode, actualCommandCode);
        }

        [Fact]
        public void Execute_WhenCalled_MultipleTimes_CallSerialPortOpen_Once()
        {
            Init();

            var command = new CommandRead(0xf1, 1);

            Sut.Execute(command);
            Sut.Execute(command);

            FakeSerialPortDecorator.Received(1).Open();
        }

        [Fact]
        public void Execute_ForEachCall_CallSerialPortWrite()
        {
            Init();

            var command = new CommandRead(0xf1, 1);

            Sut.Execute(command);
            Sut.Execute(command);

            FakeSerialPortDecorator.Received(2).Write(Arg.Any<IEnumerable<byte>>());
        }

        [Fact]
        public void Dispose_WhenCalled_CallDisposeOnSerialPort()
        {
            Init();

            var command = new CommandRead(0xf1, 1);

            Sut.Execute(command);

            Sut.Dispose();
            FakeSerialPortDecorator.Received(1).Dispose();
        }

        [Fact]
        public void Execute_WhenCalled_ReturnValidResponse()
        {
            Init();

            var expectedResponse = new List<byte> {0x55, 0x56};
            var overflow = new List<byte> {0x99, 0xFF};
            var fakeResponse = new NcdApiProtocol(expectedResponse).GetApiEncodedData().ToList();
            fakeResponse.AddRange(overflow);
            var command = new CommandRead(0xf1, 1);


            var fakeSerialPortDecorator = Substitute.ForPartsOf<FakeSerialPortDecorator>();
            fakeSerialPortDecorator.GetResponse().Returns(fakeResponse);
            Sut = new ApiToSerialBridge(new SerialGateway(fakeSerialPortDecorator));

            var response = Sut.Execute(command);

            Assert.Equal(expectedResponse, response.Payload);
            Assert.True(response.IsValid);
        }
    }

    [ExcludeFromCodeCoverage]
    public class FakeSerialPortDecorator : ISerialPortDecorator
    {
        public virtual IEnumerable<byte> GetResponse() => new List<byte> {0x00};

        public void Dispose()
        {
        }

        public event EventHandler<DataReceivedArgs> DataReceived;

        public void Open()
        {
        }

        public void Write(IEnumerable<byte> byteSequence)
        {
            // Response is passed as DataReceived.
            DataReceived?.Invoke(this, new DataReceivedArgs {Data = GetResponse().ToArray()});
        }
    }
}