using System;
using System.Collections.Generic;
using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Settings;

namespace Modules
{
    // ReSharper disable once UnusedMember.Global
    public class AMS5812_0150_D_B_Module : BaseModuleV2
    {
        public override byte DefaultAddress => 0x78;

        public AMS5812_0150_D_B_Module(ISerialConverter serialPort) : base(serialPort)
        { }

        public float Pressure { get; protected set; }

        public float PressureCorrection { get; set; }

        public float Temperature { get; protected set; }

        protected override IEnumerable<RegisterBase> Registers => new List<RegisterBase>(0);

        public int DevicePressureMin = 3277;
        public int DevicePressureMax = 29491;

        public float OutputPressureMin = -1034f;
        public float OutputPressureMax = 1034;

        public int DeviceTempMin = 3277;
        public int DeviceTempMax = 29491;

        public float OutputTempMin = -25f;
        public float OutputTempMax = 85f;

        public void ReadFromDevice()
        {
            var command = new ReadCommand { Address = Address, LengthRequested = 4 };
            var response = SerialPort.Execute(command);

            var measuredPressure = response.Payload[0] * 256 + response.Payload[1];

            Pressure = (float)Math.Round(
                (measuredPressure - DevicePressureMin) *
                (OutputPressureMax - OutputPressureMin) /
                (DevicePressureMax - DevicePressureMin) +
                OutputPressureMin,
                2);

            var measuredTemp = response.Payload[2] * 256 + response.Payload[3];

            Temperature = (float)Math.Round(
                (measuredTemp - DeviceTempMin) *
                (OutputTempMax - OutputTempMin) /
                (DeviceTempMax - DeviceTempMin) +
                OutputTempMin,
                2);
        }

        public override void Init()
        {
            // No initialization required.
        }
    }
}
