using BoosterPumpLibrary.Contracts;
using System;
using System.Collections.Generic;

namespace Modules
{
    using BoosterPumpLibrary.ModuleBase;
    using BoosterPumpLibrary.Settings;

    public class As1115Module : BaseModuleV2
    {
        // see:https://s3.amazonaws.com/controleverything.media/controleverything/Production%20Run%2013/45_AS1115_I2CL_3CE_AMB/Datasheets/AS1115_Datasheet_EN_v2.pdf

        private readonly BoosterPumpLibrary.Settings.Register Digits = new BoosterPumpLibrary.Settings.Register(0x01, "Digits", 3);

        private BitSetting Digit0 => Digits.GetOrCreateSubRegister(8, 0, "Digit0");
        private BitSetting Digit1 => Digits.GetOrCreateSubRegister(8, 8, "Digit0");
        private BitSetting Digit2 => Digits.GetOrCreateSubRegister(8, 16, "Digit0");
               
        /// <summary>
        /// Register 0x09..0x0C
        /// </summary>
        private readonly BoosterPumpLibrary.Settings.Register Settings1 = new BoosterPumpLibrary.Settings.Register(0x09, "Settings 0x09..0x0C", 4);

        /// <summary>
        /// Nubmer: 0..7 - then numbers binary representation 0b000..0b111 switch the digits on/off.
        /// </summary>
        private BitSetting DecodeMode => Settings1.GetOrCreateSubRegister(3, 0, "Decode Mode");

        /// <summary>
        /// 0..15
        /// </summary>
        private BitSetting GlobalIntensity => Settings1.GetOrCreateSubRegister(4, 0 + 8, "Global Intensity");

        /// <summary>
        /// 0: Digit 0, 1: Digit 0..1, 2:Digit 0..2
        /// </summary>
        private BitSetting ScanLimit => Settings1.GetOrCreateSubRegister(3, 0 + 32, "Scan digits");

        /// <summary>
        /// 0x00: Shutdown Mode, reset feature registers.
        /// 0x70: Shutdown Mode, feature registers unchanged.
        /// 0x01: Normal Operation, reset feature registers to default settings.
        /// 0x71: Normal Operation, feature registers unchanged.
        /// </summary>
        private BitSetting Shutdonw => Settings1.GetOrCreateSubRegister(8, 0 + 40, "Shutdown Mode");

        /// <summary>
        /// 0x0E..0x11
        /// </summary>
        private readonly BoosterPumpLibrary.Settings.Register Settings2 = new BoosterPumpLibrary.Settings.Register(0x0E, "Settings 0x0e..0x11", 4);

        /// <summary>
        /// 0: BCD decoding.
        /// 1: HEX decoding.
        /// </summary>
        private BitSetting DecodeSelection => Settings2.GetOrCreateSubRegister(1, 2, "Decode Dec/Hex");

        /// <summary>
        /// 0bX0: no blinking
        /// 0b01: blinking with 1 Hz
        /// 0b11: blinking with 0.5 Hz
        /// </summary>
        private BitSetting Blink => Settings2.GetOrCreateSubRegister(2, 4, "Blink settings");

        /// <summary>
        /// 0..15
        /// </summary>
        private BitSetting Digit0Intensity => Settings2.GetOrCreateSubRegister(4, 16, "Digit 0 intensity");

        /// <summary>
        /// 0..15
        /// </summary>
        private BitSetting Digit1Intensity => Settings2.GetOrCreateSubRegister(4, 4 + 16, "Digit 1 intensity");

        /// <summary>
        /// 0..15
        /// </summary>
        private BitSetting Digit2Intensity => Settings2.GetOrCreateSubRegister(4, 0 + 24, "Digit 2 intensity");

        public override byte DefaultAddress => 0x00;

        public As1115Module(ISerialConverter serialPort) : base(serialPort)
        { }

        protected override IEnumerable<RegisterBase> Registers => new[] { Settings1, Settings2 };

        public override void Init()
        {
            SetPrimarySettingsDirty();
            SetShutdownModeNormalResetFeature();
            SetBcdDecoding();
            SetGlobalIntensity(0x0F);
            SetDigitsVisible(0x03);
        }

        protected void SetAllDecodeOn()
        {
            DecodeMode.Value = 0xb111;
        }

        public void SetNoDecoding()
        {
            DecodeMode.Value = 0xb000;
        }

        public void SetBcdDecoding()
        {
            SetAllDecodeOn();
            DecodeSelection.Value = 0;
        }

        public void SetHexDecoding()
        {
            SetAllDecodeOn();
            DecodeSelection.Value = 1;
        }

        public void BlinkFast()
        {
            Blink.Value = 0b01;
        }

        public void BlinkOff()
        {
            Blink.Value = 0b00;
        }

        public void BlinkSlow()
        {
            Blink.Value = 0xb11;
        }

        /// <summary>
        /// Set intensity value, range 0x00 ... 0x0F.
        /// </summary>
        /// <param name="value"></param>
        public void SetGlobalIntensity(byte value)
        {
            GlobalIntensity.Value = value;
        }

        public void SetDigit0Intensity(byte value)
        {
            Digit0Intensity.Value = value;
        }

        public void SetDigit1Intensity(byte value)
        {
            Digit1Intensity.Value = value;
        }

        public void SetDigit2Intensity(byte value)
        {
            Digit2Intensity.Value = value;
        }

        /// <summary>
        /// Number of visible digits 0..2 => 1..3
        /// </summary>
        /// <param name="value"></param>
        public void SetDigitsVisible(byte value)
        {
            ScanLimit.Value = value;
        }

        public void SetShutdownModeNormalResetFeature()
        {
            Shutdonw.Value = 0x01;
        }

        public void SetShutdownModeDown()
        {
            Shutdonw.Value = 0x00;
        }

        public void SetPrimarySettingsDirty()
        {
            var registers = new[] { Settings1, Settings2 };

            foreach (var register in registers)
            {
                register.SetDirty();
            }
        }

        /// <summary>
        /// -99 .. -0.1, 000, 0.01...999
        /// Range 0.01..999 only total of 3 digits.
        /// </summary>
        /// <param name="value"></param>
        public void SetBcdValue(float value)
        {
            if (value < -99 || 999 < value)
            {
                Digit0.Value =0x0B; // 'E'
                Digit1.Value =0x0B; // 'E'
                Digit2.Value =0x0B; // 'E'
            }
            // -99 ... -10
            else if (value < -9.95)
            {
                Digit0.Value =0x0A; // '-'
                var digit1 = (byte)Math.Abs(value / 10);
                var digit2 = (byte)Math.Abs(value % 10);
                Digit1.Value =digit1;
                Digit2.Value =digit2;
            }
            // -9.9 ... -0.1
            else if (value < -0.095)
            {
                Digit0.Value =0x0A; // '-'
                var digit1 = (byte)Math.Abs(value);
                var digit2 = (byte)Math.Abs(value * 10 % 10);
                Digit1.Value =(byte)(digit1 | 0b1000_0000);
                Digit2.Value =digit2;
            }
            // -0.09 ... 0.005 => 0.00
            else if (value < 0.005)
            {
                Digit0.Value =0x00 | 0b1000_0000;
                Digit1.Value =0x00;
                Digit2.Value =0x00;
            }
            // 0.01 ... 9.99
            else if (value < 10)
            {
                Digit0.Value =(byte)((byte)value | 0b1000_0000);
                Digit1.Value =(byte)(value * 10 % 10);
                Digit2.Value =(byte)(value * 100 % 10);
            }
            // 10.0 ... 99.9
            else if (value < 100)
            {
                Digit0.Value =(byte)((byte)value / 10);
                Digit1.Value =(byte)((byte)(value % 10) | 0b1000_0000);
                Digit2.Value =(byte)(value * 10 % 10);
            }
            // 100 ... 999
            else
            {
                Digit0.Value =(byte)(value / 100);
                Digit1.Value =(byte)(value / 10 % 10);
                Digit2.Value =(byte)(value % 10);
            }

            Send();
        }

        /// <summary>
        /// Set display value from Hex Source, 0x00..0x0F, set decimal dot by adding 0xb1000_000.
        /// </summary>
        /// <param name="value"></param>
        public void SetHexValue(byte[] value)
        {
            Digit0.Value =value[0];
            Digit1.Value =value[1];
            Digit2.Value =value[2];
        }
    }
}
