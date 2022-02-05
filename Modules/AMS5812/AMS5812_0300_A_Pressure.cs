﻿using eu.iamia.NCD.API.Contract;

namespace Modules
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    public class AMS5812_0300_A_Pressure : AMS5812_0150_D_Pressure
    {
        protected override float OutputPressureMax => 2068f;
        protected override float OutputPressureMin => 0f;

        public AMS5812_0300_A_Pressure(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        { }
    }
}