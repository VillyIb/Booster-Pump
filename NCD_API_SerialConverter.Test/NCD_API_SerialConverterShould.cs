using System.Collections.Generic;
using System.Linq;
using BoosterPumpLibrary.Commands;
using NCD_API_SerialConverter.Contracts;
using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;
using NSubstitute;
using Xunit;

namespace NCD_API_SerialConverter.Test
{
    public class NcdApiSerialConverterShould
    {
        private readonly INcdApiSerialPort FakeSerialPort;
        private readonly SerialConverter Sut;

        private byte Address { get; set; }

        public NcdApiSerialConverterShould()
        {
            FakeSerialPort = Substitute.For<INcdApiSerialPort>();
            Sut = new SerialConverter(FakeSerialPort);

            Address = 0x41;
        }

        [Fact]
        public void WriteByteSequenceForExecuteReadCommand()
        {
            var command = new ReadCommand { DeviceAddress = Address, LengthRequested = 1 };
            Sut.Execute(command);

            var expected = new byte[] { 0xAA, 0x03, 0xBF, 0x41, 0x01, 0xAE };

            FakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Length == seq.Count()));
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }

        [Fact]
        public void WriteByteSequenceForExecuteWriteCommand()
        {
            var command = new WriteCommand { DeviceAddress = Address, Payload = new byte[] { 0x03, 0x00 } };
            Sut.Execute(command);

            var expected = new byte[] { 0xAA, 0x04, 0xBE, 0x41, 0x03, 0x00, 0xB0 };

            FakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Length == seq.Count()));
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }

        [Fact]
        public void WriteByteSequenceForExecuteWriteReadCommand()
        {
            Address = 0x68;
            var command = new WriteReadCommand { DeviceAddress = Address, Payload = new byte[] { 0x10 }, Delay = 0x16, LengthRequested = 0x02 };
            Sut.Execute(command);

            var expected = new byte[] { 0xAA, 0x05, 0xC0, 0x68, 0x02, 0x16, 0x10, 0xFF };

            FakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Length == seq.Count()));
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }

        [Fact]
        public void WriteByteSequenceForScanCommand()
        {
            var ncdCommand = new ConverterScan();
            Sut.Execute(ncdCommand);

            var expected = new byte[] { 0xAA, 0x02, 0xC1, 0x00, 0x6D };

            Assert.True( expected.SequenceEqual(ncdCommand.ApiEncodedData()));

            FakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Length == seq.Count()));
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }

        [Fact]
        public void WriteByteSequenceForSoftRebootCommand()
        {
            var ncdCommand = new ConverterSoftReboot();
            Sut.Execute(ncdCommand);

            var expected = new byte[] { 0xAA, 0x03, 0xFE, 0x21, 0xBC, 0x88 };

            Assert.True(expected.SequenceEqual(ncdCommand.ApiEncodedData()));

            FakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Length == seq.Count()));
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }

        [Fact]
        public void WriteByteSequenceForHardRebootCommand()
        {
            var ncdCommand = new ConverterHardReboot();
            Sut.Execute(ncdCommand);

            var expected = new byte[] { 0xAA, 0x03, 0xFE, 0x21, 0xBD, 0x89 };

            Assert.True(expected.SequenceEqual(ncdCommand.ApiEncodedData()));

            FakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Length == seq.Count()));
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }

        [Fact]
        public void WriteByteSequenceForStopCommand()
        {
            var ncdCommand = new ConverterStop();
            Sut.Execute(ncdCommand);

            var expected = new byte[] { 0xAA, 0x03, 0xFE, 0x21, 0xBB, 0x87 };

            Assert.True(expected.SequenceEqual(ncdCommand.ApiEncodedData()));

            FakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Length == seq.Count()));
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }

        [Fact]
        public void WriteByteSequenceForTest2WayCommand()
        {
            var ncdCommand = new ConverterTest2Way();
            Sut.Execute(ncdCommand);

            var expected = new byte[] { 0xAA, 0x02, 0xFE, 0x21, 0xCB };

            Assert.True(expected.SequenceEqual(ncdCommand.ApiEncodedData()));

            FakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Length == seq.Count()));
            FakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }
    }
}
