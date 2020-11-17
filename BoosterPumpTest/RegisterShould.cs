using System;
using System.Collections.Generic;
using System.Text;
using BoosterPumpLibrary.Settings;
using NSubstitute;
using Xunit;

namespace BoosterPumpTest
{
    public class RegisterShould
    {
        public Register sut;

        public BitSetting BsA;
        public BitSetting BsB;
        public BitSetting BsC;
        public BitSetting BsD;

        public RegisterShould()
        {
            BsA = new BitSetting { Size = 1, StartPositon = 0 }; // 0
            BsB = new BitSetting { Size = 2, StartPositon = 1 }; // 1..2
            BsC = new BitSetting { Size = 3, StartPositon = 3 }; // 3..6
            BsD = new BitSetting { Size = 1, StartPositon = 7 }; // 7

            sut = new Register(0x00, "test register", new[] { BsA, BsB, BsC, BsD });
        }

        [Fact]
        public void HaveSpecificValueSettingBsA ()
        {
            BsA.Value = 1;
            Assert.Equal(0b000_0001, sut.Value);
        }

        [Fact]
        public void HaveSpecificValueSettingBsB()
        {
            BsB.Value = 3;
            Assert.Equal(0b000_0110, sut.Value);
        }

        [Fact]
        public void HaveSpecificValueSettingBsC()
        {
            BsC.Value = 7;
            Assert.Equal(0b011_1000, sut.Value);
        }

        [Fact]
        public void HaveSpecificValueSettingBsD()
        {
            BsD.Value = 1;
            Assert.Equal(0b100_0000, sut.Value);
        }
    }
}
