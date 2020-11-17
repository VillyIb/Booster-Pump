using BoosterPumpLibrary.Contracts;

using BoosterPumpLibrary.Settings;
using System;
using System.Collections.Generic;

namespace BoosterPumpLibrary.Modules
{
    public class MCP4725_4_20mA_CurrentTransmitterV2 : BoosterPumpLibrary.ModuleBase.BaseModule
    {
        // Product, see: https://store.ncd.io/product/1-channel-4-20ma-current-loop-transmitter-i2c-mini-module/
        // Datasheet see: https://media.ncd.io/sites/2/20170721135048/MCP4725.pdf
        
        public override byte DefaultAddress => 0x60; // optional 0x61

        private  BitSetting PowerDown = new BitSetting(1, 1 + 16);

        private  BitSetting WriteToDacOrEeprom = new BitSetting(2, 4 +16);

        private  BitSetting Speed = new BitSetting(12, 4);



        private Register Setting;

        protected new IEnumerable<Register> Registers => new[] { Setting };

        public override void Init()
        {
            SetNormalPower();
            SetSpeedPersistent(0.50f);
                    }

        public MCP4725_4_20mA_CurrentTransmitterV2(ISerialConverter serialPort) : base(serialPort)
        {
            Setting = new Register(0, "Settings", 3, new[] { PowerDown, WriteToDacOrEeprom, Speed });
        }

        public void SetNormalPower()
        {
            PowerDown.Value = 0;
        }

        /// <summary>
        /// Grounds outut with 1k resistor.
        /// </summary>
        public void SetPowerDown()
        {
            PowerDown.Value = 1;
            Send();
        }

        protected void WriteToDacOnly()
        {
            PowerDown.Value = 2;
        }

        protected void WriteToDacAndEeprom()
        {
            PowerDown.Value = 3;
        }

        protected void SetSpeed(ulong speed)
        {
            Speed.Value = speed;            
            Send();
        }

        /// <summary>
        /// Set speed in pct of max {0,0...100,0}
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeed(float speed)
        {
            WriteToDacOnly();
            SetSpeed((ulong)GetIntValue(speed));
        }

        /// <summary>
        /// Speed is in the ragne 0..100 with 12 bit resolution.
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeedPersistent(float speed)
        {
            WriteToDacAndEeprom();
            SetSpeed((ulong)GetIntValue(speed));
        }

        public float GetPctValute(int value)
        {
            if(value < 0 || 4096 < value) { throw new ArgumentOutOfRangeException(nameof(value), value, "Valid: 0...4096 (int)" ); }

            var dec = value / 4096.0f; //  4096 = 2**12)
            return dec;
        }

        public int GetIntValue(float value)
        {
            if (value < 0.0f || 1.0f < value) { throw new ArgumentOutOfRangeException(nameof(value), value, "Valid: 0...1 (float)"); }

            return (int)Math.Round(value * 4096f, 0);
        }



    }
}
