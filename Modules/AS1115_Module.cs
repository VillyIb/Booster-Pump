﻿using BoosterPumpLibrary.Contracts;
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
        private readonly BoosterPumpLibrary.Settings.Register Setting0x09 = new BoosterPumpLibrary.Settings.Register(0x09, "Settings 0x09", 1);

        /// <summary>
        /// Nubmer: 0..7 - then numbers binary representation 0b000..0b111 switch the digits on/off.
        /// </summary>
        private BitSetting DecodeMode => Setting0x09.GetOrCreateSubRegister(3, 0, "Decode Mode");

        private readonly BoosterPumpLibrary.Settings.Register Setting0x0A = new BoosterPumpLibrary.Settings.Register(0x0A, "Settings 0x0A", 1);

        /// <summary>
        /// 0..15
        /// </summary>
        private BitSetting GlobalIntensity => Setting0x0A.GetOrCreateSubRegister(4, 0, "Global Intensity");

        private readonly BoosterPumpLibrary.Settings.Register Setting0x0B = new BoosterPumpLibrary.Settings.Register(0x0B, "Settings 0x0B", 1);

        /// <summary>
        /// 0: Digit 0, 1: Digit 0..1, 2:Digit 0..2
        /// </summary>
        private BitSetting ScanLimit => Setting0x0B.GetOrCreateSubRegister(3, 0, "Scan digits");

        private readonly BoosterPumpLibrary.Settings.Register Setting0x0C = new BoosterPumpLibrary.Settings.Register(0x0C, "Settings 0x0C", 1);

        /// <summary>
        /// 0x00: Shutdown Mode, reset feature registers.
        /// 0x70: Shutdown Mode, feature registers unchanged.
        /// 0x01: Normal Operation, reset feature registers to default settings.
        /// 0x71: Normal Operation, feature registers unchanged.
        /// </summary>
        private BitSetting Shutdonw => Setting0x0C.GetOrCreateSubRegister(8, 0, "Shutdown Mode");

        /// <summary>
        /// 0x0E..0x11
        /// </summary>
        private readonly BoosterPumpLibrary.Settings.Register Setting0x0E = new BoosterPumpLibrary.Settings.Register(0x0E, "Settings 0x0E", 1);

        /// <summary>
        /// 0: BCD decoding.
        /// 1: HEX decoding.
        /// </summary>
        private BitSetting DecodeSelection => Setting0x0E.GetOrCreateSubRegister(1, 2, "Decode Dec/Hex");

        /// <summary>
        /// 0bX0: no blinking
        /// 0b01: blinking with 1 Hz
        /// 0b11: blinking with 0.5 Hz
        /// </summary>
        private BitSetting Blink => Setting0x0E.GetOrCreateSubRegister(2, 4, "Blink settings");

        private readonly BoosterPumpLibrary.Settings.Register Setting0x10 = new BoosterPumpLibrary.Settings.Register(0x10, "Settings 0x0e..0x11", 1);

        /// <summary>
        /// 0..15
        /// </summary>
        private BitSetting Digit0Intensity => Setting0x10.GetOrCreateSubRegister(4, 0, "Digit 0 intensity");

        /// <summary>
        /// 0..15
        /// </summary>
        private BitSetting Digit1Intensity => Setting0x10.GetOrCreateSubRegister(4, 4, "Digit 1 intensity");

        private readonly BoosterPumpLibrary.Settings.Register Setting0x11 = new BoosterPumpLibrary.Settings.Register(0x11, "Settings 0x0e..0x11", 1);

        /// <summary>
        /// 0..15
        /// </summary>
        private BitSetting Digit2Intensity => Setting0x11.GetOrCreateSubRegister(4, 0, "Digit 2 intensity");

        public override byte DefaultAddress => 0x00;

        public As1115Module(ISerialConverter serialPort) : base(serialPort)
        { }

        protected override IEnumerable<RegisterBase> Registers => new[] {
            // Notice the order is important!
            Setting0x0C,
            Setting0x0E,

            Setting0x09, 
            Setting0x0A, 
            Setting0x0B, 

            Setting0x10, 
            Setting0x11,
            Digits
        };

        public override void Init()
        {
            SetPrimarySettingsDirty();
            SetShutdownModeNormalResetFeature();
            SetBcdDecoding();
            SetGlobalIntensity(15);
            SetDigitsVisible(3);
        }

        protected void SetAllDecodeOn()
        {
            DecodeMode.Value = 0b111;
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
        /// Number of visible digits 1..3
        /// </summary>
        /// <param name="value"></param>
        public void SetDigitsVisible(byte value)
        {
            ScanLimit.Value = (ulong)(value-1);
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
            var registers = new[] {
            Setting0x0C,
            Setting0x0E,
            Setting0x09,
            Setting0x0A,
            Setting0x0B,
        };

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