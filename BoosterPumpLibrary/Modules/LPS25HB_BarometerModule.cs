using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.ModuleBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoosterPumpLibrary.Modules
{
    public class LPS25HB_BarometerModule : BaseModule
    {
        public override byte Address => 0x5C;

        Register Res_conf = new Register(0x10, "Pressure and temperature resolution", "X2");

        Register Ctrl_reg1 = new Register(0x20, "Control register 1", "X2");

        Register Press_out_XL = new Register(0x28, "Pressure out value (LSB)", "X2");
        Register Press_out_L = new Register(0x29, "Pressure out value (mid part)", "X2");
        Register Press_out_H = new Register(0x2A, "Pressure out value (MSB)", "X2");
        Register Who_am_I = new Register(0x0F, "Who am I", "X2");


        public double AirPressure { get; protected set; }

        public double Temperature { get; protected set; }

        protected override IEnumerable<Register> Registers => new[] { Res_conf, Ctrl_reg1 };

        public LPS25HB_BarometerModule(ISerialConverter serialPort) : base(serialPort)
        { }


        public override void Init()
        {
            Ctrl_reg1.SetDataRegisterBit(BitPattern.D7, true); // Power: active mode
            Ctrl_reg1.SetDataRegisterBit(BitPattern.D4, true); // Output data rate 1Hz
            Ctrl_reg1.SetDataRegisterBit(BitPattern.D2, true); // BDU output registers not updated until MSB and LSB have been read

            Send();
        }

        public void ReadDevice()
        {
            {
                SetRegisterForReading(Who_am_I);
                var readCommand = new ReadCommand { Address = Address, LengthRequested = 1 };
                var readings = SerialPort.Execute(readCommand);
            }

            {
                SetRegisterForReading(Press_out_XL);
                var readCommand = new ReadCommand { Address = Address, LengthRequested = 5 };
                var readings = SerialPort.Execute(readCommand);

                AirPressure = (readings.Payload[0] + readings.Payload[1] << 8 + readings.Payload[2] << 16) / 4096.0;

                Temperature = (readings.Payload[3] + readings.Payload[4] << 8) / 480.0;
            }
        }
    }
}
