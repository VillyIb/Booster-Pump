using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoosterPumpLibrary.Settings;
using Xunit;

namespace BoosterPumpTest
{
public    class BitSettingsShould
    {

        [Fact]
        public void X()
        {
            var sut = new BitSetting(4, 2, "MyDescription");
            Assert.Equal(4,sut.Size);
            Assert.Equal(0x02, sut.Offset);
            Assert.Equal("MyDescription", sut.Description);
            Assert.Equal("1111_00", sut.MaskAsBinary());
            Assert.Equal(15ul, sut.Mask);
            Assert.Equal("",sut.ToString());
        }


    }
}
