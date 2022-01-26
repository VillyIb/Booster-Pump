// ReSharper disable InconsistentNaming


namespace Modules
{
    using BoosterPumpLibrary.Settings;
    using System;
    using System.Collections.Generic;
    using eu.iamia.NCD.API;
    using eu.iamia.NCD.API.Contract;

    public class LPS25HB_Barometer : InputModule
    {
        // see: https://store.ncd.io/product/lps25hb-mems-pressure-sensor-260-1260-hpa-absolute-digital-output-barometer-i2c-mini-module/
        // Description see: https://media.ncd.io/sites/2/20170721134650/LPS25hb.pdf
        // Technical note regarding calculating values.
        // see: https://www.st.com/resource/en/technical_note/dm00242307-how-to-interpret-pressure-and-temperature-readings-in-the-lps25hb-pressure-sensor-stmicroelectronics.pdf

        public static byte DefaultAddressValue => 0x5C;

        #region Utils

        private const double TemperatureSensitivity = 480.0;

        private const double TemperatureOffset = 42.5; // from technical note.

        internal static double MapTemperature(short hex)
        {
            var result = TemperatureOffset + hex / TemperatureSensitivity;
            return result;
        }

        public static short MapTemperature(double temperature)
        {
            return (short)((temperature - TemperatureOffset) * TemperatureSensitivity);
        }

        private const double PressureSensitivity = 4096.0;

        private const double PressureOffset = 0;

        internal static double MapPressure(int hex)
        {
            var result = PressureOffset + hex / PressureSensitivity;
            return result;
        }

        public static int MapPressure(double Pressure)
        {
            return (int)((Pressure - PressureOffset) * PressureSensitivity);
        }

        #endregion

        #region Settings

        public override byte DefaultAddress => DefaultAddressValue;

        /// <summary>
        /// see: Description 8.5 Res_Conf Pressure and temperature resolution.
        /// </summary>
        private readonly Register Settings0X10 = new(0X10, "RES_CONF", 1, Direction.Output);

        /// <summary>
        /// 0: 8, 1: 16, 2: 32, 3: 64 internal average.
        /// </summary>
        private UInt8BitSettingsWrapper PressureResolution => new(Settings0X10.GetOrCreateSubRegister(2, 0, "Pressure Resolution"));

        /// <summary>
        /// 0: 8, 1: 16, 2: 32, 3: 64 internal average.
        /// </summary>
        private UInt8BitSettingsWrapper TemperatureResolution => new(Settings0X10.GetOrCreateSubRegister(2, 2, "Temperature Resolution"));

        /// <summary>
        /// Control register 1
        /// </summary>
        private readonly Register Settings0X20 = new(0x20, "CTRL_REG1", 1, Direction.Output);

        /// <summary>
        /// 0: Power Down, 1: Active Mode.
        /// </summary>
        private UInt8BitSettingsWrapper PowerDown => new(Settings0X20.GetOrCreateSubRegister(1, 7, "Power down control"));

        /// <summary>
        /// 0: One Shot, 1: 1 Hz, 2: 7 Hz, 3: 12,5 Hz, 4: 25Hz, 5..7 Reserved.
        /// </summary>
        private UInt8BitSettingsWrapper OutputDataRate => new(Settings0X20.GetOrCreateSubRegister(3, 4, "Output data rate."));

        #endregion

        internal readonly Register Reading0X28 = new(0x28, "Air Pressure & Temperature", 5, Direction.Input);

        private Int24BitSettingsWrapper PressureHex => new(Reading0X28.GetOrCreateSubRegister(24, 0, "Barometric Pressure"));

        private Int16BitSettingsWrapper TemperatureHex => new(Reading0X28.GetOrCreateSubRegister(16, 24, "Air Temperature"));


        public double AirPressure
        {
            get => Reading0X28.IsInputDirty
                    ? double.NaN
                    : Math.Round(MapPressure(PressureHex.Value), 2);
            internal set => PressureHex.Value = MapPressure(value);
        }

        public double Temperature
        {
            get => Reading0X28.IsInputDirty
                    ? double.NaN
                    : Math.Round(MapTemperature(TemperatureHex.Value), 2);

            internal set => TemperatureHex.Value = MapTemperature(value);
        }

        protected override IEnumerable<Register> Registers => new List<Register> { Settings0X20, Settings0X10, Reading0X28 };

        public LPS25HB_Barometer(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        { }

        public virtual void Init()
        {
            PowerDown.Value = 1;
            OutputDataRate.Value = 1;
            PressureResolution.Value = 2;
            TemperatureResolution.Value = 2;
            Send();

            SendSpecificRegister(Reading0X28);
            var readCommand = new CommandRead(DeviceAddress, 5);
            // ReSharper disable once UnusedVariable
            var returnValue = ApiToSerialBridge.Execute(readCommand);
        }

        public override void ReadFromDevice()
        {
            Reading0X28.IsInputDirty = true;
            base.ReadFromDevice();
        }

        public override bool IsInputValid => !Reading0X28.IsInputDirty;
    }
}
