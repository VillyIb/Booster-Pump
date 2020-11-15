using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.ModuleBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoosterPumpLibrary.Modules
{
    public class MCP4725_4_20mA_CurrentTransmitter : BaseModule
    {
        // Product, see: https://store.ncd.io/product/1-channel-4-20ma-current-loop-transmitter-i2c-mini-module/
        // Datasheet see: https://media.ncd.io/sites/2/20170721135048/MCP4725.pdf

        
        public override byte Address => 0x60; // optional 0x61

        private readonly Register Byte2 = new Register(0x00, "Byte2", "X2");
        private readonly Register Byte3 = new Register(0x01, "Byte3", "X2");
        private readonly Register Byte4 = new Register(0x02, "Byte4", "X2");

        protected override IEnumerable<Register> Registers => new[] { Byte2, Byte3, Byte4 };

        public override void Init()
        {
            SetSpeed(0.50f);
        }

        public MCP4725_4_20mA_CurrentTransmitter(ISerialConverter serialPort) : base(serialPort)
        { }

        public void SetNormalPower()
        {
            Byte2.SetDataRegisterBit(BitPattern.D1, false); // PD0
            Byte2.SetDataRegisterBit(BitPattern.D2, false); // PD1
        }

        /// <summary>
        /// Grounds outut with 1k resistor.
        /// </summary>
        public void SetPowerDown()
        {
            Byte2.SetDataRegisterBit(BitPattern.D1, true); // PD0
            Byte2.SetDataRegisterBit(BitPattern.D2, false); // PD1
            Send();
        }

        protected void WriteToDacOnly()
        {
            Byte2.SetDataRegisterBit(BitPattern.D5, false); // C0
            Byte2.SetDataRegisterBit(BitPattern.D6, true); // C1
        }

        protected void WriteToDacAndEeprom()
        {
            Byte2.SetDataRegisterBit(BitPattern.D5, true); // C0
            Byte2.SetDataRegisterBit(BitPattern.D6, true); // C1
        }

        protected void SetSpeed(short speed)
        {
            // LSB is written to Byte3
            // MSB bits are writtent to Byte2 4 lower bit

            var lsb = (byte)((speed & 0xf) << 4);
            Byte4.SetDataRegister(lsb);
            var msb = (byte)((speed & 0x0ff0) >> 4);
            Byte3.SetDataRegister(msb);
            
            Send();
        }

        /// <summary>
        /// Set speed in pct of max {0,0...100,0}
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeed(float speed)
        {
            WriteToDacOnly();
            SetSpeed((short)GetIntValue(speed));
        }

        /// <summary>
        /// Speed is in the ragne 0..100 with 12 bit resolution.
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeedPersistent(float speed)
        {
            WriteToDacAndEeprom();
            SetSpeed((short)GetIntValue(speed));
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
