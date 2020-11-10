using System;
using System.Collections.Generic;
using System.Text;
using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.Modules;
using NSubstitute;
using NSubstitute.Routing.Handlers;
using Xunit;

namespace BoosterPumpTest
{
    public class AMS5812_0150_D_B_ModuleShould
    {
        private readonly AMS5812_0150_D_B_Module _Sut;
        private readonly ISerialConverter _FakeSerialPort;

        public AMS5812_0150_D_B_ModuleShould()
        {
            _FakeSerialPort = Substitute.For<ISerialConverter>();
            _Sut = new AMS5812_0150_D_B_Module(_FakeSerialPort);
        }

        [Fact]
        public void SendReadSequenceCallingReadFromDevice()
        {
            _Sut.ReadFromDevice();

            //_FakeSerialPort.When()...ReturnAutoValue(0xAA, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00); // simulate measurement...

            _FakeSerialPort.Received().Execute(Arg.Is<WriteCommand>(c => c.I2CDataAsHex == "78 00"));

            Assert.Equal(55.5f, _Sut.Pressure); 
            Assert.Equal(24.5f, _Sut.Temperature);
        }

    }
}
