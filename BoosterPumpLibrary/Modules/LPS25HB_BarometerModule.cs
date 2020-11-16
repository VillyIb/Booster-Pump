using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.ModuleBase;
using System;
using System.Collections.Generic;

namespace BoosterPumpLibrary.Modules
{
    public class LPS25HB_BarometerModule : BaseModule
    {
        // Description see: https://media.ncd.io/sites/2/20170721134650/LPS25hb.pdf
        // Technical note regarding calculating values.
        // see: https://www.st.com/resource/en/technical_note/dm00242307-how-to-interpret-pressure-and-temperature-readings-in-the-lps25hb-pressure-sensor-stmicroelectronics.pdf

        public override byte DefaultAddress => 0x5C;

        Register RES_CONF = new Register(0x10, "Pressure and temperature resolution", "X2");

        Register CTRL_REG1 = new Register(0x20, "Control register 1", "X2");

        Register PRESS_OUT_XL = new Register(0x28, "Pressure out value (LSB)", "X2");
        Register PRESS_OUT_L = new Register(0x29, "Pressure out value (mid part)", "X2");
        Register PRES_OUT_H = new Register(0x2A, "Pressure out value (MSB)", "X2");

        Register TEMP_OUT_L = new Register(0x2B, "Temperature (LSB)", "X2");
        Register TEMP_OUT_H = new Register(0x2C, "Temperature (MSB)", "X2");

        Register WHO_AM_I = new Register(0x0F, "Who am I", "X2");


        public double AirPressure { get; protected set; }

        public double Temperature { get; protected set; }

        protected override IEnumerable<Register> Registers => new[] { RES_CONF, CTRL_REG1 };

        public LPS25HB_BarometerModule(ISerialConverter serialPort, int? addressIncrement = 0) : base(serialPort, addressIncrement)
        { }

        public override void Init()
        {
            CTRL_REG1.SetDataRegisterBit(BitPattern.D7, true); // Power: active mode
            CTRL_REG1.SetDataRegisterBit(BitPattern.D4, true); // Output data rate 1Hz

            RES_CONF.SetDataRegisterBit(BitPattern.D0 | BitPattern.D1, true); // Pressure resolution configuration nr. internal avarage: 512 

            Send();

            SelectRegisterForReadingWithAutoIncrement(PRESS_OUT_XL);
            var readCommand = new ReadCommand { Address = Address, LengthRequested = 5 };
            var result = SerialPort.Execute(readCommand);
        }

        public void ReadDevice()
        {
            SelectRegisterForReadingWithAutoIncrement(PRESS_OUT_XL);
            var readCommand = new ReadCommand { Address = Address, LengthRequested = 5 };
            var readings = SerialPort.Execute(readCommand);

            var payload = readings.Payload;
            try
            {
                AirPressure = Math.Round((payload[0] | payload[1] << 8 | payload[2] << 16) / 4096.0, 1);
                Temperature = Math.Round(42.5 + ((short)(payload[3] | payload[4] << 8)) / 480.0, 1);
            }
            catch(Exception ex)
            {

            }
        }
    }
}
