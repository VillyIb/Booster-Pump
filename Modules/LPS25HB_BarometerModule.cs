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

        readonly Register RES_CONF_V2 = new Register(0X10, "Register1", 1);

        /// <summary>
        /// 0: 8-, 1: 16, 2: 32, 3: 64 internal average.
        /// </summary>
        BitSetting PressureResolution => RES_CONF_V2.GetOrCreateSubRegister(2, 0, "Pressure Resolution");

        /// <summary>
        /// 0: 8, 1: 16, 2: 32, 3: 64 internal average.
        /// </summary>
         BitSetting TemperaturResolution => RES_CONF_V2.GetOrCreateSubRegister(2, 2, "Temperature Resolution");

        readonly Register ControlRegister = new Register(0x20, "ControlRegister", 1);

        /// <summary>
        /// 0: Power Down, 1: Active Mode.
        /// </summary>
         BitSetting PowerDown => ControlRegister.GetOrCreateSubRegister(1, 7, "Power down control");

        /// <summary>
        /// 0: One Shot, 1: 1 Hz, 2: 7 Hz, 3: 12,5 Hz, 4: 25Hz, 5..7 Reserved.
        /// </summary>
        BitSetting OutputDataRate => ControlRegister.GetOrCreateSubRegister(3, 4, "Output data rate.");

        readonly Register Reading = new Register(0x28, "Air Pressure & Temperatur", 5);

        public double AirPressure { get; protected set; }

        public double Temperature { get; protected set; }

        protected override IEnumerable<RegisterBase> Registers => new List<RegisterBase>();

        public LPS25HB_BarometerModule(ISerialConverter serialPort, int? addressIncrement = 0) : base(serialPort, addressIncrement)
        {
                                
        }

        public override void Init()
        {
            PowerDown.Value = 1;
            OutputDataRate.Value = 1;
            PressureResolution.Value = 3;
            Send();

            SelectRegisterForReadingWithAutoIncrement(Reading);
            var readCommand = new ReadCommand { Address = Address, LengthRequested = 5 };
            var result = SerialPort.Execute(readCommand);
        }

        public void ReadDevice()
        {
            SelectRegisterForReadingWithAutoIncrement(Reading);
            var readCommand = new ReadCommand { Address = Address, LengthRequested = (byte)Reading.Size };
            var readings = SerialPort.Execute(readCommand);
            var payload = readings.Payload;
            try
            {
                AirPressure = Math.Round((payload[0] | payload[1] << 8 | payload[2] << 16) / 4096.0, 1);
                Temperature = Math.Round(42.5 + (short)(payload[3] | payload[4] << 8) / 480.0, 1);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {

            }
        }
    }
}
