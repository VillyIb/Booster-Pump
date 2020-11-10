using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace SerialConverter.Test
{
    using NSubstitute;
    using NCD_API_SerialConverter.Contracts;
    using BoosterPumpLibrary.Commands;
    using NCD_API_SerialConverter.Commands;
    using NCD_API_SerialConverter;
    using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;

    public class NCD_API_SerialConverterShould
    {
        INCD_API_SerialPort fakeSerialPort;
        SerialConverter sud;

        private byte Address { get; set; }

        public NCD_API_SerialConverterShould()
        {
            fakeSerialPort = Substitute.For<INCD_API_SerialPort>();
            sud = new SerialConverter(fakeSerialPort);

            Address = 0x41;
        }

        [Fact]
        public void WriteByteSequenceForEcecuteReadCommand()
        {
            var command = new ReadCommand { Address = Address, LengthRequested = 1 };
            sud.Execute(command);

            var expected = new byte[] { 0xAA, 0x03, 0xBF, 0x41, 0x01, 0xAE };

            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Count() == seq.Count()));
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }

        [Fact]
        public void WriteByteSequenceForEcecuteWriteCommand()
        {
            var command = new WriteCommand { Address = Address, Payload = new byte[] { 0x03, 0x00 } };
            sud.Execute(command);

            var expected = (new byte[] { 0xAA, 0x04, 0xBE, 0x41, 0x03, 0x00, 0xB0 });

            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Count() == seq.Count()));
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }

        [Fact]
        public void WriteByteSequenceForEcecuteWriteReadCommand()
        {
            Address = 0x68;
            var command = new WriteReadCommand { Address = Address, Payload = new byte[] { 0x10 }, Delay = 0x16, LengthRequested = 0x02 };
            sud.Execute(command);

            var expected = (new byte[] { 0xAA, 0x05, 0xC0, 0x68, 0x02, 0x16, 0x10, 0xFF });

            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Count() == seq.Count()));
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }

        [Fact]
        public void WriteByteSequenceForScanCommand()
        {
            var ncdCommand = new ConverterScan();
            sud.Execute(ncdCommand);

            var expected = (new byte[] { 0xAA, 0x02, 0xC1, 0x00, 0x6D });

            expected.SequenceEqual(ncdCommand.ApiEncodedData());

            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Count() == seq.Count()));
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }

        [Fact]
        public void WriteByteSequenceForSoftRebootCommand()
        {
            var ncdCommand = new ConverterSoftReboot();
            sud.Execute(ncdCommand);

            var expected = (new byte[] { 0xAA, 0x03, 0xFE, 0x21, 0xBC, 0x88 });

            expected.SequenceEqual(ncdCommand.ApiEncodedData());

            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Count() == seq.Count()));
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }

        [Fact]
        public void WriteByteSequenceForHardRebootCommand()
        {
            var ncdCommand = new ConverterHardReboot();
            sud.Execute(ncdCommand);

            var expected = (new byte[] { 0xAA, 0x03, 0xFE, 0x21, 0xBD, 0x89 });

            expected.SequenceEqual(ncdCommand.ApiEncodedData());

            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Count() == seq.Count()));
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }

        [Fact]
        public void WriteByteSequenceForStopCommand()
        {
            var ncdCommand = new ConverterStop();
            sud.Execute(ncdCommand);

            var expected = (new byte[] { 0xAA, 0x03, 0xFE, 0x21, 0xBB, 0x87 });

            expected.SequenceEqual(ncdCommand.ApiEncodedData());

            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Count() == seq.Count()));
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }

        [Fact]
        public void WriteByteSequenceForTest2WayCommand()
        {
            var ncdCommand = new ConverterTest2Way();
            sud.Execute(ncdCommand);

            var expected = (new byte[] { 0xAA, 0x02, 0xFE, 0x21, 0xCB });

            expected.SequenceEqual(ncdCommand.ApiEncodedData());

            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.Count() == seq.Count()));
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => expected.SequenceEqual(seq)));
        }
    }
}
