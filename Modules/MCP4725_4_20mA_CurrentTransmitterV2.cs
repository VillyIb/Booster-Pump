﻿using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.Settings;
using System;
using System.Collections.Generic;

namespace Modules
{
    // ReSharper disable once InconsistentNaming
    public class MCP4725_4_20mA_CurrentTransmitterV2 : BoosterPumpLibrary.ModuleBase.BaseModuleV2
    {
        // Product, see: https://store.ncd.io/product/1-channel-4-20ma-current-loop-transmitter-i2c-mini-module/
        // Datashet see: https://media.ncd.io/sites/2/20170721135048/MCP4725.pdf

        public override byte DefaultAddress => 0x60; // optional 0x61

        /// <summary>
        /// 0: normal mode, 1: 1 kOhm-, 2 100 kOmh-, 3: 500 kOhm resitor to ground.
        /// See table 5-2
        /// </summary>
        private readonly BitSetting PowerDown;

        /// <summary>
        /// C0 and C1, 0: or 1: FastMode (not supported), 2: Write to DAC register. 3: Write to DAC register and EEPROM.
        /// See table 6-2
        /// </summary>
        private readonly BitSetting WriteToDacOrEeprom;

        /// <summary>
        /// 12 bit floating point value. (0..4095).
        /// </summary>
        private readonly BitSetting Speed;

        private readonly Register Setting;

        protected override IEnumerable<RegisterBase> Registers => new[] { Setting };

        public override void Init()
        {
            SetNormalPower();
            SetSpeedPersistent(0.50f);
        }

        public MCP4725_4_20mA_CurrentTransmitterV2(ISerialConverter serialPort) : base(serialPort)
        {
            Setting = new Register(0, "Settings", 3);
            PowerDown = Setting.CreateSubRegister(1, 1 + 16, "Power Down");
            WriteToDacOrEeprom = Setting.CreateSubRegister(2, 4 + 16, "Write to DAC or EEPROM");
            Speed = Setting.CreateSubRegister(12, 4, "Speed");
        }

        public void SetNormalPower()
        {
            PowerDown.Value = 0;
        }

        /// <summary>
        /// Grounds output with 1k resistor.
        /// </summary>
        public void SetPowerDown()
        {
            PowerDown.Value = 1;
            Send();
        }

        protected void WriteToDacOnly()
        {
            WriteToDacOrEeprom.Value = 2;
        }

        protected void WriteToDacAndEeprom()
        {
            WriteToDacOrEeprom.Value = 3;
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
        /// Speed is in the range [0..1[ with 12 bit resolution ~ approx 4 digits.
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeedPersistent(float speed)
        {
            WriteToDacAndEeprom();
            SetSpeed((ulong)GetIntValue(speed));
        }

        public float GetPctValue(int value)
        {
            if (value < 0 || 4096 <= value) { throw new ArgumentOutOfRangeException(nameof(value), value, "Valid: [0...4096[ (int)"); }

            var dec = value / 4096.0f; //  4096 = 2**12)
            return dec;
        }

        public int GetIntValue(float value)
        {
            if (value < 0.0f || 1.0f <= value) { throw new ArgumentOutOfRangeException(nameof(value), value, "Valid: [0...1[ (float)"); }

            return (int)Math.Round(value * 4096f, 0);
        }
    }
}
