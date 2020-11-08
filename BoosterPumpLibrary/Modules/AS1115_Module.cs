using BoosterPumpLibrary.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoosterPumpLibrary.Modules
{
    using ModuleBase;
    using Commands;
    
    public class AS1115_Module
    {
        // see:https://s3.amazonaws.com/controleverything.media/controleverything/Production%20Run%2013/45_AS1115_I2CL_3CE_AMB/Datasheets/AS1115_Datasheet_EN_v2.pdf

        private readonly IModuleCommunication serialPort;

        private Register RegDigit0 = new Register(0x01, "Digit 0", "N0");
        private Register RegDigit1 = new Register(0x02, "Digit 1", "N0");
        private Register RegDigit2 = new Register(0x03, "Digit 2", "N0");

        private Register RegDecodingEnabled = new Register(0x09, "DecodingEnabled", "X");
        private Register RegGlobalIntensityRegister = new Register(0x0A, "GlobalIntensity", "N0");
        private Register RegScanLimit = new Register(0x0B, "ScanLimit", "N0");

        private Register RegShutdownRegister = new Register(0x0C, "ShutDownRegister", "N0");
        private Register RegFeatureRegister = new Register(0x0E, "FeatureRegister", "N0");
        private Register RegDisplayTestMode = new Register(0x0F, "DisplayTestMode", "N0");

        private Register RegIntensityDigit01 = new Register(0x10, "IntensityDigit01", "N0");
        private Register RegIntensityDigit23 = new Register(0x11, "IntensityDigit23", "N0");

        public byte Address => 0x00;

        private AS1115_Module()
        { }

        public AS1115_Module(IModuleCommunication serialPort)
        {
            this.serialPort = serialPort;
        }

        Register[] registers => new[] { RegShutdownRegister, RegFeatureRegister, RegDecodingEnabled, RegGlobalIntensityRegister, RegScanLimit, RegIntensityDigit01, RegIntensityDigit23, RegDigit0, RegDigit1, RegDigit2 };

        /// <summary>
        /// Returns next command for each call.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> CurrentCommand()
        {
            var result = new List<byte>();
            byte currentRegisterId = 0;

            foreach (var current in registers)
            {

                if (current.IsDirty)
                {
                    if (result.Count > 0 && (currentRegisterId + 1) != current.RegisterId) { break; }
                    if (result.Count == 0)
                    {
                        result.Add(current.RegisterId);
                    }
                    result.Add(current.GetDataRegisterAndClearDirty());
                    currentRegisterId = current.RegisterId;
                }
            }

            return result;
        }

        public bool MoveNextCommand()
        {
            var next = registers.FirstOrDefault(t => t.IsDirty);
            return registers.Any(t => t.IsDirty);
        }

        public void Send()
        {
            while (MoveNextCommand())
            {
                var output = new List<byte>();
                output.Add(Address);
                var currentCommand = CurrentCommand();
                output.AddRange(currentCommand);

                var writeCommand = new WriteCommand { Address = Address, Payload = currentCommand };

                serialPort.Execute(writeCommand);
            }
        }

        /// <summary>
        ///  Sendes initialization sequence
        /// </summary>
        public void Init()
        {
            SetPrimarySettingsDirty();
            SetShutdownModeNormalResetFeature();
            SetBcdDecoding();
            SetGlobalIntensity(0x0F);
            SetDigitsVisible(0x03);
        }

        protected void SetAllDecodeOn()
        {
            RegDecodingEnabled.SetDataRegisterBit(BitPattern.D0, true);
            RegDecodingEnabled.SetDataRegisterBit(BitPattern.D1, true);
            RegDecodingEnabled.SetDataRegisterBit(BitPattern.D2, true);
        }

        public void SetNoDecoding()
        {
            RegDecodingEnabled.SetDataRegisterBit(BitPattern.D0, false);
            RegDecodingEnabled.SetDataRegisterBit(BitPattern.D1, false);
            RegDecodingEnabled.SetDataRegisterBit(BitPattern.D2, false);
        }

        public void SetBcdDecoding()
        {
            SetAllDecodeOn();
            RegFeatureRegister.SetDataRegisterBit(BitPattern.D2, false);
        }

        public void SetHexDecoding()
        {
            SetAllDecodeOn();
            RegFeatureRegister.SetDataRegisterBit(BitPattern.D2, true);
        }

        public void BlinkFast()
        {
            RegFeatureRegister.SetDataRegisterBit(BitPattern.D4, true);
            RegFeatureRegister.SetDataRegisterBit(BitPattern.D5, false);
        }

        public void BlinkOff()
        {
            RegFeatureRegister.SetDataRegisterBit(BitPattern.D4, false);
            RegFeatureRegister.SetDataRegisterBit(BitPattern.D5, false);
        }

        public void BlinkSlow()
        {
            RegFeatureRegister.SetDataRegisterBit(BitPattern.D4, true);
            RegFeatureRegister.SetDataRegisterBit(BitPattern.D5, true);
        }

        /// <summary>
        /// Set intensitity value, range 0x00 ... 0x0F.
        /// </summary>
        /// <param name="value"></param>
        public void SetGlobalIntensity(byte value)
        {
            this.RegGlobalIntensityRegister.SetDataRegister((byte)(value & 0x0f));
        }

        public void Digit0Intensity(byte value)
        {
            RegIntensityDigit01.SetDataRegister((byte)(RegIntensityDigit01.Value & 0xf0 | value & 0x0f));
        }

        public void Digit1Intensity(byte value)
        {
            RegIntensityDigit01.SetDataRegister((byte)(RegIntensityDigit01.Value & 0x0f | value << 4 & 0xf0));
        }

        public void Digit2Intensity(byte value)
        {
            RegIntensityDigit23.SetDataRegister((byte)(RegIntensityDigit23.Value & 0xf0 | value & 0x0f));
        }

        /// <summary>
        /// Number of visible digits 0..2 => 1..3
        /// </summary>
        /// <param name="value"></param>
        public void SetDigitsVisible(byte value)
        {
            RegScanLimit.SetDataRegister((byte)(value - 1 & 0b0000_0011));
        }

        public void SetShutdownModeNormalResetFeature()
        {
            RegShutdownRegister.SetDataRegister(0b0000_0001);
        }

        public void SetShutdownModeDown()
        {
            RegShutdownRegister.SetDataRegister(0b0000_0000);
        }

        public void SetPrimarySettingsDirty()
        {
            var registers = new[] { RegShutdownRegister, RegFeatureRegister, RegDecodingEnabled, RegGlobalIntensityRegister, RegScanLimit };

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
                RegDigit0.SetDataRegister(0x0B); // 'E'
                RegDigit1.SetDataRegister(0x0B); // 'E'
                RegDigit2.SetDataRegister(0x0B); // 'E'
            }
            // -99 ... -10
            else if (value < -9.95)
            {
                RegDigit0.SetDataRegister(0x0A); // '-'
                var digit1 = (byte)Math.Abs(value / 10);
                var digit2 = (byte)Math.Abs(value % 10);
                RegDigit1.SetDataRegister(digit1);
                RegDigit2.SetDataRegister(digit2);
            }
            // -9.9 ... -0.1
            else if (value < -0.095)
            {
                RegDigit0.SetDataRegister(0x0A); // '-'
                var digit1 = (byte)Math.Abs(value);
                var digit2 = (byte)Math.Abs(value * 10 % 10);
                RegDigit1.SetDataRegister((byte)(digit1 | 0b1000_0000));
                RegDigit2.SetDataRegister(digit2);
            }
            // -0.09 ... 0.005 => 0.00
            else if (value < 0.005)
            {
                RegDigit0.SetDataRegister((byte)(0x00 | 0b1000_0000));
                RegDigit1.SetDataRegister(0x00);
                RegDigit2.SetDataRegister(0x00);
            }
            // 0.01 ... 9.99
            else if (value < 10)
            {
                RegDigit0.SetDataRegister((byte)(((byte)value) | 0b1000_0000));
                RegDigit1.SetDataRegister((byte)(value * 10 % 10));
                RegDigit2.SetDataRegister((byte)(value * 100 % 10));
            }
            // 10.0 ... 99.9
            else if (value < 100)
            {
                RegDigit0.SetDataRegister((byte)((byte)value / 10));
                RegDigit1.SetDataRegister((byte)((byte)(value % 10) | 0b1000_0000));
                RegDigit2.SetDataRegister((byte)(value * 10 % 10));
            }
            // 100 ... 999
            else
            {
                RegDigit0.SetDataRegister((byte)(value / 100));
                RegDigit1.SetDataRegister((byte)(value / 10 % 10));
                RegDigit2.SetDataRegister((byte)(value % 10));
            }

            Send();
        }

        /// <summary>
        /// Set display value from Hex Source, 0x00..0x0F, set decimal dot by adding 0xb1000_000.
        /// </summary>
        /// <param name="value"></param>
        public void SetHexVaue(byte[] value)
        {
            RegDigit0.SetDataRegister(value[0]);
            RegDigit1.SetDataRegister(value[1]);
            RegDigit2.SetDataRegister(value[2]);
        }

    }
}
