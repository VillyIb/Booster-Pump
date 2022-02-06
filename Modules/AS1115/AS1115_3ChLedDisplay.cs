using System;
using System.Collections.Generic;
using System.Linq;
using BoosterPumpLibrary.Settings;
using EnsureThat;
using eu.iamia.BaseModule.Contract;
using eu.iamia.i2c.communication.contract;
using eu.iamia.Util.Extensions;

namespace Modules.AS1115
{
    /// <summary>
    /// Display module 3 digits
    /// </summary>
    public class As1115Module 
    {
        private readonly IOutputModule ComModule;
        // see:https://s3.amazonaws.com/controleverything.media/controleverything/Production%20Run%2013/45_AS1115_I2CL_3CE_AMB/Datasheets/AS1115_Datasheet_EN_v2.pdf
        // see:https://store.ncd.io/product/7-segment-3-character-led-display-i2c-mini-module/

        public enum Level0xF
        {
            Level0,
            Level1,
            Level2,
            Level3,
            Level4,
            Level5,
            Level6,
            Level7,
            Level8,
            Level9,
            LevelA,
            LevelB,
            LevelC,
            LevelD,
            LevelE,
            LevelF
        }

        #region Feature Register 0x01 .. 0x02

        private readonly Register Digits = new(0x01, "Digits", 3, Direction.Output);

        private UInt8BitSettingsWrapper Digit0 => new(Digits.GetOrCreateSubRegister(8, 16, "Digit0"));
        private UInt8BitSettingsWrapper Digit1 => new(Digits.GetOrCreateSubRegister(8, 8, "Digit1"));
        private UInt8BitSettingsWrapper Digit2 => new(Digits.GetOrCreateSubRegister(8, 0, "Digit2"));

        #endregion

        #region Feature Register 0x09

        /// <summary>
        /// Off: 8 bits maps to individual line segments (table 11)
        /// On:  7 bits is decoded as BCD or Hex + comma (table 9 or 10)
        /// </summary>
        [Flags]
        public enum DecodeModeSettings
        {
            AllDigitsOff = 0x00,
            Digit1On = 0b0000_0001,
            Digit2On = 0b0000_0010,
            Digit3On = 0b0000_0100,
            AllDigitsOn = 0b0000_0111
        }

        /// <summary>
        /// Register 0x09..0x0C
        /// </summary>
        private readonly Register Setting0X09 = new(0x09, "Settings 0x09", 1, Direction.Output);

        /// <summary>
        /// Number: 0..7 - then numbers binary representation 0b000..0b111 switch the digits on/off.
        /// </summary>
        public EnumBitSettings<DecodeModeSettings> DecodeMode => new(Setting0X09.GetOrCreateSubRegister(3, 0, "Decode Mode"));

        #endregion

        #region Feature Register 0x0A

        private readonly Register Setting0X0A = new(0x0A, "Settings 0x0A", 1, Direction.Output);

        /// <summary>
        /// 0..15
        /// </summary>
        public EnumBitSettings<Level0xF> GlobalIntensity => new(Setting0X0A.GetOrCreateSubRegister(4, 0, "Global Intensity"));

        #endregion

        #region Feature Register 0x0B

        private readonly Register Setting0X0B = new(0x0B, "Settings 0x0B", 1, Direction.Output);

        public enum ActiveDigits
        {
            First = 0,
            FirstToSecond = 1,
            FirstToThird = 2

        }
        /// <summary>
        /// 0: Digit 0, 1: Digit 0..1, 2:Digit 0..2
        /// </summary>
        public EnumBitSettings<ActiveDigits> ScanLimit => new(Setting0X0B.GetOrCreateSubRegister(3, 0, "Scan digits"));

        #endregion

        #region Feature Register 0x0C

        private readonly Register Setting0X0C = new(0x0C, "Settings 0x0C", 1, Direction.Output);

        public enum ShutdownRegisterSettings
        {
            ShutdownModeResetFeatureRegisterToDefaultSettings = 0x00,

            ShutdownModeFeatureRegisterUnchanged = 0x80,

            NormalOperationResetFeatureRegisterToDefaultSettings = 0x01,

            NormalOperationFeatureRegisterUnchanged = 0x81
        }

        /// <summary>
        /// 0x00: ShutdownRegister Mode, reset feature registers.
        /// 0x70: ShutdownRegister Mode, feature registers unchanged.
        /// 0x01: Normal Operation, reset feature registers to default settings.
        /// 0x71: Normal Operation, feature registers unchanged.
        /// </summary>
        public EnumBitSettings<ShutdownRegisterSettings> ShutdownRegister => new(Setting0X0C.GetOrCreateSubRegister(8, 0, "ShutdownRegister Mode"));


        #endregion

        #region Feature Register 0x0E

        /// <summary>
        /// { BCD | HEX }
        /// </summary>
        public enum Decoding
        {
            BCD = 0,

            HEX = 1
        }

        /// <summary>
        /// {NoFlash | Flash1H | Flash05hz }
        /// </summary>
        public enum FlashMode
        {
            NoFlash = 0,

            /// <summary>
            /// 1 Hz.
            /// </summary>
            FlashFast = 1,

            /// <summary>
            /// 0.5 Hz.
            /// </summary>
            FlashSlow = 3
        }

        /// <summary>
        /// 0x0E..0x11
        /// </summary>
        private readonly Register Setting0X0E = new(0x0E, "Settings 0x0E", 1, Direction.Output);

        /// <summary>
        /// 0: BCD decoding.
        /// 1: HEX decoding.
        /// </summary>
        public EnumBitSettings<Decoding> DecodingSetting => new(Setting0X0E.GetOrCreateSubRegister(1, 2, "Decode Dec/Hex"));

        /// <summary>
        /// 0bX0: no blinking
        /// 0b01: blinking with 1 Hz
        /// 0b11: blinking with 0.5 Hz
        /// </summary>
        public EnumBitSettings<FlashMode> Blink => new(Setting0X0E.GetOrCreateSubRegister(2, 4, "Blink settings"));

        #endregion

        #region Feature Register 0x10, 0x11

        private readonly Register Setting0X10 = new(0x10, "Settings 0x10", 2, Direction.Output);

        /// <summary>
        /// 0..15
        /// </summary>
        public EnumBitSettings<Level0xF> Digit0Intensity => new(Setting0X10.GetOrCreateSubRegister(4, 0, "Digit 0 intensity"));

        /// <summary>
        /// 0..15
        /// </summary>
        public EnumBitSettings<Level0xF> Digit1Intensity => new(Setting0X10.GetOrCreateSubRegister(4, 4, "Digit 1 intensity"));

        /// <summary>
        /// 0..15
        /// </summary>
        public EnumBitSettings<Level0xF> Digit2Intensity => new(Setting0X10.GetOrCreateSubRegister(4, 8, "Digit 2 intensity"));

        #endregion

        public static byte DefaultAddressValue => 0x00;


        public As1115Module(IOutputModule comModule)
        {
            ComModule = comModule;
            ComModule.SetupOnlyOnce(Registers, DefaultAddressValue);
        }

        protected IEnumerable<IRegister> Registers => new[] {
            // Notice the order is important!
            Setting0X0C,
            Setting0X0E,

            Setting0X09,
            Setting0X0A,
            Setting0X0B,

            Setting0X10,
            Digits
        };

        public virtual void Init()
        {
            DecodeMode.Value = DecodeModeSettings.AllDigitsOn;
            SetPrimarySettingsDirty();
            ShutdownRegister.Value = ShutdownRegisterSettings.NormalOperationResetFeatureRegisterToDefaultSettings;
            DecodingSetting.Value = Decoding.BCD;
            GlobalIntensity.Value = Level0xF.LevelF;
            ScanLimit.Value = ActiveDigits.FirstToThird;
        }

        public void SetPrimarySettingsDirty()
        {
            var registers = new[] {
            Setting0X0C,
            Setting0X0E,
            Setting0X09,
            Setting0X0A,
            Setting0X0B,
        };

            foreach (var register in registers)
            {
                register.IsOutputDirty = true;
            }
        }

        /// <summary>
        /// -99 .. -0.1, 000, 0.01...999
        /// Range 0.01..999 only total of 3 digits.
        /// </summary>
        /// <param name="value"></param>
        public void SetBcdValue(float value)
        {
            // ReSharper disable once MergeIntoLogicalPattern
            if (value.IsOutsideRange(-99, 999))
            {
                Digit0.Value = 0x0B; // 'E'
                Digit1.Value = 0x0B; // 'E'
                Digit2.Value = 0x0B; // 'E'
            }
            // -99 ... -10
            else if (value < -9.95)
            {
                Digit0.Value = 0x0A; // '-'
                var digit1 = (byte)Math.Abs(value / 10);
                var digit2 = (byte)Math.Abs(value % 10);
                Digit1.Value = digit1;
                Digit2.Value = digit2;
            }
            // -9.9 ... -0.1
            else if (value < -0.095)
            {
                Digit0.Value = 0x0A; // '-'
                var digit1 = (byte)Math.Abs(value);
                var digit2 = (byte)Math.Abs(value * 10 % 10);
                Digit1.Value = (byte)(digit1 | 0b1000_0000);
                Digit2.Value = digit2;
            }
            // -0.09 ... 0.005 => 0.00
            else if (value < 0.005)
            {
                Digit0.Value = 0x00 | 0b1000_0000;
                Digit1.Value = 0x00;
                Digit2.Value = 0x00;
            }
            // 0.01 ... 9.99
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            else if (value < 10)
            {
                Digit0.Value = (byte)((byte)value | 0b1000_0000);
                Digit1.Value = (byte)(value * 10 % 10);
                Digit2.Value = (byte)(value * 100 % 10);
            }
            // 10.0 ... 99.9
            else if (value < 100)
            {
                Digit0.Value = (byte)((byte)value / 10);
                Digit1.Value = (byte)((byte)(value % 10) | 0b1000_0000);
                Digit2.Value = (byte)(value * 10 % 10);
            }
            // 100 ... 999
            else
            {
                Digit0.Value = (byte)(value / 100);
                Digit1.Value = (byte)(value / 10 % 10);
                Digit2.Value = (byte)(value % 10);
            }

            ComModule.Send();
        }

        /// <summary>
        /// Set display value from Hex Source, 0x00..0x0F, set decimal dot by adding 0b1000_000.
        /// </summary>
        /// <param name="value"></param>
        public void SetHexValue(byte[] value)
        {
            Ensure.That(value, nameof(value)).IsNotNull();
            Ensure.That(value.ToList(), nameof(value)).SizeIs(3);

            Digit0.Value = value[0];
            Digit1.Value = value[1];
            Digit2.Value = value[2];
        }

        public void Send()
        {
            ComModule.Send();
        }
    }
}
