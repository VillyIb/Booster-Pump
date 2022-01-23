using System;
using System.Collections.Generic;
using System.Xml;
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

        public static uint DevicePressureMin => 3277;
        public static uint DevicePressureMax => 29491;

        public virtual float OutputPressureMin => -1034f;
        public virtual float OutputPressureMax => 1034f;

        public static uint DeviceTempMin => 3277;
        public static uint DeviceTempMax => 29491;

        public virtual float OutputTempMin => -25f;
        public virtual float OutputTempMax => 85f;

        public float Pressure => Readings.IsInputDirty
            ? float.NaN
            : (float)Math.Round(
                (PressureHex.Value - DevicePressureMin) *
                (OutputPressureMax - OutputPressureMin) /
                (DevicePressureMax - DevicePressureMin) +
                OutputPressureMin,
                2);

        public float Temperature => Readings.IsInputDirty
            ? float.NaN
            : (float)Math.Round(
                (TemperatureHex.Value - DeviceTempMin) *
                (OutputTempMax - OutputTempMin) /
                (DeviceTempMax - DeviceTempMin) +
                OutputTempMin,
                2);

        private void Clear()
        {
            Readings.SetInputDirty();
        }

        private readonly Register Readings = new(0x78, "Readings", 4);

        protected override IEnumerable<Register> Registers => new[]
        {
            Readings
        };

        private BitSetting TemperatureHex => Readings.GetOrCreateSubRegister(16, 0, "Temperature");

        private BitSetting PressureHex => Readings.GetOrCreateSubRegister(16, 16, "Pressure");

        /// <summary>
        /// Pressure module
        /// </summary>
        /// <param name="apiToSerialBridge"></param>
        public AMS5812_0150_D_Pressure(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        {
            Clear();
        }

        public override bool IsInputValid =>
            float.IsFinite(Temperature)
            &&
            Temperature.IsWithinRange(OutputTempMin, OutputTempMax)
            &&
            float.IsFinite(Pressure)
            && Pressure.IsWithinRange(OutputPressureMin, OutputPressureMax)
            ;

        public override void ReadFromDevice()
        {
            Clear();
            base.ReadFromDevice();
        }
    }
}
