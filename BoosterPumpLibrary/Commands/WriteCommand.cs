﻿using System;
using System.Collections.Generic;

namespace BoosterPumpLibrary.Commands
{
    [Obsolete("Use: namespace eu.iamia.NCD.API.WriteCommand")]
    public class WriteCommand : CommandBase
    {
        public IEnumerable<byte> Payload { get; set; }

        public WriteCommand(byte deviceAddress, IEnumerable<byte> payload)
            : base(deviceAddress)
        {
            Payload = payload;
        }

        public override IEnumerable<byte> I2C_Data()
        {
            yield return DeviceAddress;
            foreach (var current in Payload)
            {
                yield return current;
            }
        }
    }
}
