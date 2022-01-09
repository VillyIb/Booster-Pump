using System;
using System.Collections.Generic;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Settings;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;
using eu.iamia.Util.Extensions;

namespace Modules
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InconsistentNaming
    // see: https://store.ncd.io/product/ams5812-0150-d-b-amplified-pressure-sensor-1034-to-1034-mbar-15-to-15-psi-i2c-mini-module/

    public class AMS5812_0150_D_Pressure : InputModule
    {
        public static byte DefaultAddressValue => 0x78;

        public override byte DefaultAddress => DefaultAddressValue;

        public virtual byte LengthRequested => 0x04;

        public static int DevicePressureMin => 3277;
        public static int DevicePressureMax => 29491;

        public virtual float OutputPressureMin => -1034f;
        public virtual float OutputPressureMax => 1034f;

        public static int DeviceTempMin => 3277;
        public static int DeviceTempMax => 29491;

        public virtual float OutputTempMin => -25f;
        public virtual float OutputTempMax => 85f;

        public float Pressure { get; protected set; }

        public float PressureCorrection { get; set; }

        public float Temperature { get; protected set; }

        private void ClearOutput()
        {
            Temperature = float.NaN;
            Pressure = float.NaN;
        }

        protected override IEnumerable<Register> Registers => new List<Register>(0); // TODO right to use '=>'

        /// <summary>
        /// Pressure module
        /// </summary>
        /// <param name="apiToSerialBridge"></param>
        public AMS5812_0150_D_Pressure(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        {
            ClearOutput();
        }

        public override bool IsInputValid =>
            float.IsFinite(Temperature)
            &&
            Temperature.IsWithinRange(OutputPressureMin, OutputPressureMax)
            &&
            float.IsFinite(Pressure)
            && Pressure.IsWithinRange(OutputPressureMin, OutputPressureMin)
            ;

        public override bool IsOutputValid => true; // TODO fix real value

        // TODO 
        public override void ReadFromDevice()
        {
            ClearOutput();

            var command = new CommandRead(DeviceAddress, LengthRequested);

            var response = ApiToSerialBridge.Execute(command);

            if (response is null)
            {
                return;
            }

            if (!response.IsValid)
            {
                return;
            }

            if (response.IsError)
            {
                return;
            }

            var measuredPressure = response.Payload[0] << 8 | response.Payload[1];

            Pressure = (float)Math.Round(
                (measuredPressure - DevicePressureMin) *
                (OutputPressureMax - OutputPressureMin) /
                (DevicePressureMax - DevicePressureMin) +
                OutputPressureMin,
                2);

            var measuredTemp = response.Payload[2] << 8 | response.Payload[3];

            Temperature = (float)Math.Round(
                (measuredTemp - DeviceTempMin) *
                (OutputTempMax - OutputTempMin) /
                (DeviceTempMax - DeviceTempMin) +
                OutputTempMin,
                2);
        }
    }
}
