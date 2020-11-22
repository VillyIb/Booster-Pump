using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Settings;
using System;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace Modules
{
    public class LPS25HB_BarometerModule : BaseModuleV2
    {
        // Description see: https://media.ncd.io/sites/2/20170721134650/LPS25hb.pdf
        // Technical note regarding calculating values.
        // see: https://www.st.com/resource/en/technical_note/dm00242307-how-to-interpret-pressure-and-temperature-readings-in-the-lps25hb-pressure-sensor-stmicroelectronics.pdf

        public override byte DefaultAddress => 0x5C;


        private readonly Register Settings0X10 = new Register(0X10, "RES_CONF", 1);

        /// <summary>
        /// 0: 8-, 1: 16, 2: 32, 3: 64 internal average.
        /// </summary>
        private BitSetting PressureResolution => Settings0X10.GetOrCreateSubRegister(2, 0, "Pressure Resolution");

        /// <summary>
        /// 0: 8, 1: 16, 2: 32, 3: 64 internal average.
        /// </summary>
        private BitSetting TemperatureResolution => Settings0X10.GetOrCreateSubRegister(2, 2, "Temperature Resolution");


        private readonly Register Settings0X20 = new Register(0x20, "Control Register", 1);

        /// <summary>
        /// 0: Power Down, 1: Active Mode.
        /// </summary>
        private BitSetting PowerDown => Settings0X20.GetOrCreateSubRegister(1, 7, "Power down control");

        /// <summary>
        /// 0: One Shot, 1: 1 Hz, 2: 7 Hz, 3: 12,5 Hz, 4: 25Hz, 5..7 Reserved.
        /// </summary>
        private BitSetting OutputDataRate => Settings0X20.GetOrCreateSubRegister(3, 4, "Output data rate.");


        private readonly Register Reading0X28 = new Register(0x28, "Air Pressure & Temperature", 5);

        private BitSetting PressureHex => Reading0X28.GetOrCreateSubRegister(24, 0, "Barometric Pressure");

        private BitSetting TemperatureHex => Reading0X28.GetOrCreateSubRegister(16, 24, "Air Temperature");


        public double AirPressure => Math.Round(PressureHex.Value / 4096.0, 1);

        public double Temperature => Math.Round(42.5 + (short)TemperatureHex.Value / 480.0, 1);

        protected override IEnumerable<RegisterBase> Registers => new List<RegisterBase> { Settings0X20, Settings0X10 };

        public LPS25HB_BarometerModule(ISerialConverter serialPort, int? addressIncrement = 0) : base(serialPort, addressIncrement)
        { }

        public override void Init()
        {
            PowerDown.Value = 1;
            OutputDataRate.Value = 1;
            PressureResolution.Value = 2;
            TemperatureResolution.Value = 2;
            Send();

            SelectRegisterForReadingWithAutoIncrement(Reading0X28);
            var readCommand = new ReadCommand { DeviceAddress = DeviceAddress, LengthRequested = 5 };
            SerialPort.Execute(readCommand);
        }

        public void ReadDevice()
        {
            SelectRegisterForReadingWithAutoIncrement(Reading0X28);
            var readCommand = new ReadCommand { DeviceAddress = DeviceAddress, LengthRequested = (byte)Reading0X28.Size };
            var readings = SerialPort.Execute(readCommand);

            var mapped = new byte[8];
            Array.Copy(readings.Payload, 0, mapped, 0, readings.Payload.Length);
            Reading0X28.Value = BitConverter.ToUInt64(mapped, 0);
        }
    }
}
