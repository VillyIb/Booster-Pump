using System;
using System.Collections.Generic;
using BoosterPumpLibrary.Settings;
using eu.iamia.NCD.API.Contract;
using eu.iamia.Util.Extensions;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Modules.MCP4725
{
    // ReSharper disable once InconsistentNaming
    public class MCP4725_4_20mA_CurrentTransmitterV2 : BoosterPumpLibrary.ModuleBase.OutputModule
    {
        // Product, see: https://store.ncd.io/product/1-channel-4-20ma-current-loop-transmitter-i2c-mini-module/
        // DataSheet see: https://media.ncd.io/sites/2/20170721135048/MCP4725.pdf

        public static byte DefaultAddressValue => 0x60; // Optional 0x61

        public override byte DefaultAddress => DefaultAddressValue;

        #region Setting 0x00,.., 0x02

        public enum PowerDownSettings
        {
            NormalMode = 0b00,
            Resistor1k = 0b01,
            Resistor100k = 0b10,
            Resistor500k = 0b11
        }

        private readonly Register Setting = new(0, "Settings", 3, Direction.Output);

        // Values acording to figure 6-2
        // 1.byte is ignored (protocol level)
        // 2 byte: Register offset 16
        // 3 byte: Register offset 8
        // 4 byte: Register offset 0

        /// <summary>
        /// 0: normal mode, 1: 1 kOhm-, 2 100 kOmh-, 3: 500 kOhm resistor to ground.
        /// See table 5-2
        /// </summary>
        public EnumBitSettings<PowerDownSettings> PowerDown => new(Setting.GetOrCreateSubRegister(2, 1 + 16, "Power Down"));

        public enum WriteCommandType
        {
            DacOnly = 0b010,
            DacAndEEProm = 0b011
        }

        private EnumBitSettings<WriteCommandType> WriteToDacOrEEPROM => new(Setting.GetOrCreateSubRegister(3, 5 + 16, "Write to DAC or EEPROM"));

        /// <summary>
        /// 12 bit floating point value. (0..4095).
        /// </summary>
        private BitSetting Speed => Setting.GetOrCreateSubRegister(12, 4, "Speed");

        #endregion

        protected override IEnumerable<Register> Registers => new[]
        {
            Setting
        };

        public virtual void Init()
        {
            PowerDown.Value = PowerDownSettings.NormalMode;
            SetSpeedPersistent(0.50f);
        }

        public MCP4725_4_20mA_CurrentTransmitterV2(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        { }

        /// <summary>
        /// Range 0..4095, by overflow is set 4095.
        /// </summary>
        /// <param name="speed"></param>
        protected void SetSpeed(ulong speed)
        {
            Speed.Value = Math.Min(4095, speed);
            Send();
        }

        /// <summary>
        /// Set speed in pct of max {0,0...100,0}
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeed(float speed)
        {
            WriteToDacOrEEPROM.Value = WriteCommandType.DacOnly;
            SetSpeed((ulong)GetIntValue(speed));
        }

        /// <summary>
        /// Speed is in the range [0..1[ with 12 bit resolution ~ approx 4 digits.
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeedPersistent(float speed)
        {
            WriteToDacOrEEPROM.Value = WriteCommandType.DacAndEEProm;
            var speedMapped = (ulong)GetIntValue(speed);
            SetSpeed(speedMapped);
        }

        internal float GetPctValue(int value)
        {
            if (value.IsOutsideRange(0, 4096)) { throw new ArgumentOutOfRangeException(nameof(value), value, "Valid: [0...4096[ (int)"); }

            var dec = value / 4096.0f; //  4096 = 2**12)
            return dec;
        }

        internal int GetIntValue(float value)
        {
            if (value.IsOutsideRange(0.0f, 1.0f)) { throw new ArgumentOutOfRangeException(nameof(value), value, "Valid: [0...1[ (float)"); }

            return (int)Math.Round(value * 4096f, 0);
        }
    }
}
