﻿using System;
using System.Collections.Generic;

namespace BoosterPumpLibrary.Commands
{
    [Obsolete("Use: namespace eu.iamia.NCD.API.ReadCommand")]
    public class ReadCommand : CommandBase
    {
        public byte LengthRequested { get; set; }

        public ReadCommand(byte deviceAddress, byte lengthRequested)
            : base(deviceAddress)
        {
            LengthRequested = lengthRequested;
        }

        public override IEnumerable<byte> I2C_Data()
        {
            yield return DeviceAddress;
            yield return LengthRequested;
        }
    }
}
