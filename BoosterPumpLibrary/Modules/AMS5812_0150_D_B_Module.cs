using System;
using System.Collections.Generic;
using System.Text;
using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;

namespace BoosterPumpLibrary.Modules
{
    // ReSharper disable once UnusedMember.Global
    public class AMS5812_0150_D_B_Module
    {
        private readonly ISerialConverter SerialPort;

        public byte Address => 0x78;


        public AMS5812_0150_D_B_Module(ISerialConverter serialPort)
        {
            SerialPort = serialPort;
        }

        public float Pressure { get; protected set; }

        public float PressureCorrection { get; set; }

        public float Temperature { get; protected set; }



        public void ReadFromDevice()
        {
            var command = new ReadCommand{Address =  Address, LengthRequested =  4};
            var response =  SerialPort.Execute(command);

            // calculate Pressure and Temperature.
        }
    }
}
