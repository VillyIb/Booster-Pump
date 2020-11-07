using System.Linq;
using BoosterPumpLibrary.Contracts;
using NSubstitute;
using System.Collections.Generic;
using Xunit;
using BoosterPumpLibrary.Modules;

namespace BoosterPumpTest
{
    public class AS1115_ModuleShould
    {
        private readonly AS1115_Module sut;
        private readonly INCD_API_SerialPort fakeSerialPort;

        public AS1115_ModuleShould()
        {
            fakeSerialPort = Substitute.For<INCD_API_SerialPort>();
            sut = new AS1115_Module(fakeSerialPort);
        }

        [Fact]
        public void SendInitSequenceWhenCallingInitAndSend()
        {
            sut.Init();
            sut.Send();

            var byteSequence1 = new byte[] { 0xAA, 0x04, 0xBE, 0x00, 0x0C, 0x01, 0x79 };
            var byteSequence2 = new byte[] { 0xAA, 0x04, 0xBE, 0x00, 0x0E, 0x00, 0x7A };

            // register:                                                     09    0A    0B    
            var byteSequence3 = new byte[] { 0xAA, 0x06, 0xBE, 0x00, 0x09, 0x07, 0x0F, 0x02, 0x8F };

            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(s => s.SequenceEqual(byteSequence1)));
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(s => s.SequenceEqual(byteSequence2)));
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(s => s.SequenceEqual(byteSequence3)));

            Received.InOrder(() =>
            {
                fakeSerialPort.Write(Arg.Is<IEnumerable<byte>>(s => s.SequenceEqual(byteSequence1)));
                fakeSerialPort.Write(Arg.Is<IEnumerable<byte>>(s => s.SequenceEqual(byteSequence2)));
                fakeSerialPort.Write(Arg.Is<IEnumerable<byte>>(s => s.SequenceEqual(byteSequence3)));
            });
        }

        [Fact]
        public void SettingIllegalValueShowEEE()
        {
            var expected = new byte[] { 0xAA, 0x06, 0xBE, 0x00, 0x01, 0x0B, 0x0B, 0x0B, 0x90 };

            sut.SetBcdValue(-100f); // 'EEE'
            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(s => s.SequenceEqual(expected)));
        }

        [Fact]
        public void SettingMinus98ShowSame()
        {
            var expected = new byte[] { 0xAA, 0x06, 0xBE, 0x00, 0x01, 0x0A, 0x09, 0x08, 0x8A };

            sut.SetBcdValue(-98f); // '-98'
            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(s => s.SequenceEqual(expected)));

        }

        [Fact]
        public void SettingMinus7point5ValueShows_7dot5()
        {
            var expected = new byte[] { 0xAA, 0x06, 0xBE, 0x00, 0x01, 0x0A, 0x87, 0x05, 0x05 };

            sut.SetBcdValue(-7.5f); // '-7.5'
            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(s => s.SequenceEqual(expected)));
        }

        [Fact]
        public void SettingMinus75ValueShows_75()
        {
            var expected = new byte[] { 0xAA, 0x06, 0xBE, 0x00, 0x01, 0x0A, 0x07, 0x05, 0x85 };

            sut.SetBcdValue(-75f); // '-75'
            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(s => s.SequenceEqual(expected)));
        }


        [Fact]
        public void SettingZeroValueShows000()
        {
            var expected = new byte[] { 0xAA, 0x06, 0xBE, 0x00, 0x01, 0x80, 0x00, 0x00, 0xEF };

            sut.SetBcdValue(0f); // '0.00'
            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(s => s.SequenceEqual(expected)));
        }

        [Fact]
        public void Setting1dot23ValueShowsSequence()
        {
            var expected = new byte[] { 0xAA, 0x06, 0xBE, 0x00, 0x01, 0x81, 0x02, 0x03, 0xF5 };

            sut.SetBcdValue(1.23f); // '1.23'
            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(s => s.SequenceEqual(expected)));
        }

        [Fact]
        public void Setting12dot3ValueShowsSequence()
        {
            var expected = new byte[] { 0xAA, 0x06, 0xBE, 0x00, 0x01, 0x01, 0x82, 0x03, 0xF5 };

            sut.SetBcdValue(12.3f); // '12.3'
            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(s => s.SequenceEqual(expected)));
        }

        [Fact]
        public void Setting999Shows999()
        {
            var expected = new byte[] { 0xAA, 0x06, 0xBE, 0x00, 0x01, 0x09, 0x09, 0x09, 0x8A };

            sut.SetBcdValue(999f); // '999'
            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(s => s.SequenceEqual(expected)));
        }



    }
}
