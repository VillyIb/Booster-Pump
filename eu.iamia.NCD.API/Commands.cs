﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using EnsureThat;
using eu.iamia.NCD.API.Contract;
using eu.iamia.NCD.Shared;

namespace eu.iamia.NCD.API
{
    // see:https://ncd.io/serial-to-i2c-conversion/

    public abstract class Command : ICommand
    {
        public abstract IEnumerable<byte> I2C_Data();
        public abstract I2CCommandCode GetI2CCommandCode { get; }

        public string I2CDataAsHex
        {
            get
            {
                var result = new StringBuilder();

                foreach (var current in I2C_Data())
                {
                    result.AppendFormat($"{current:X2} ");
                }

                return result.ToString();
            }
        }
    }

    public abstract class CommandDevice : Command
    {
        public byte DeviceAddress { get; set; }

        protected CommandDevice(byte deviceAddress)
        {
            DeviceAddress = deviceAddress;
        }
    }

    public class CommandRead : CommandDevice
    {
        public byte LengthRequested { get; set; }

        public CommandRead(byte deviceAddress, byte lengthRequested)
            : base(deviceAddress)
        {
            LengthRequested = lengthRequested;
        }

        public override IEnumerable<byte> I2C_Data()
        {
            yield return DeviceAddress;
            yield return LengthRequested;
        }

        public override I2CCommandCode GetI2CCommandCode => I2CCommandCode.DeviceRead;
    }

    public class CommandWrite : CommandDevice
    {
        public List<byte> Payload { get; set; }

        public CommandWrite(byte deviceAddress, IEnumerable<byte> payload)
            : base(deviceAddress)
        {
            Ensure.That(payload, nameof(payload)).IsNotNull();
            Payload = payload.ToList();
            Ensure.That(Payload, nameof(payload)).SizeIs(Math.Min(255, Payload.Count));
        }

        public override IEnumerable<byte> I2C_Data()
        {
            yield return DeviceAddress;
            foreach (var current in Payload)
            {
                yield return current;
            }
        }

        public override I2CCommandCode GetI2CCommandCode => I2CCommandCode.DeviceWrite;
    }

    public class CommandWriteRead : CommandDevice
    {
        public List<byte> Payload { get; set; }

        public byte LengthRequested { get; set; }

        public byte Delay { get; set; }

        public CommandWriteRead(byte deviceAddress, IEnumerable<byte> payload, byte lengthRequested, byte delay = 0x16)
            : base(deviceAddress)
        {
            Ensure.That(payload, nameof(payload)).IsNotNull();
            Payload = payload.ToList();
            Ensure.That(Payload, nameof(payload)).SizeIs(Math.Min(255, Payload.Count));
            LengthRequested = lengthRequested;
            Delay = delay;
        }

        public override IEnumerable<byte> I2C_Data()
        {
            yield return DeviceAddress;
            yield return LengthRequested;
            yield return Delay;
            foreach (var current in Payload)
            {
                yield return current;
            }
        }

        public override I2CCommandCode GetI2CCommandCode => I2CCommandCode.DeviceWriteRead;
    }

    public abstract class CommandController : Command
    {
        private IImmutableList<byte> Payload { get; }

        protected CommandController(IEnumerable<byte> payload)
        {
            Payload = ImmutableList<byte>.Empty.AddRange(payload);
        }

        public override IEnumerable<byte> I2C_Data()
        {
            return Payload;
        }
    }

    public class CommandControllerControllerBusSCan : CommandController
    {
        public static byte[] PayloadValue = { 0x00 };

        public CommandControllerControllerBusSCan() : base(PayloadValue)
        {
        }

        public override I2CCommandCode GetI2CCommandCode => I2CCommandCode.DeviceBusScan;
    }

    public class CommandControllerControllerStop : CommandController
    {
        public static byte[] PayloadValue = { 0x21, 0xBB };

        public CommandControllerControllerStop() : base(PayloadValue)
        {
        }

        public override I2CCommandCode GetI2CCommandCode => I2CCommandCode.DeviceConverterCommand;
    }

    public class CommandControllerControllerReboot : CommandController
    {
        public static byte[] PayloadValue = { 0x21, 0xBC };

        public CommandControllerControllerReboot() : base(PayloadValue)
        {
        }

        public override I2CCommandCode GetI2CCommandCode => I2CCommandCode.DeviceConverterCommand;
    }

    public class CommandControllerControllerHardReboot : CommandController
    {
        public static byte[] PayloadValue = { 0x21, 0xBD };

        public CommandControllerControllerHardReboot() : base(PayloadValue)
        {
        }

        public override I2CCommandCode GetI2CCommandCode => I2CCommandCode.DeviceConverterCommand;
    }

    public class CommandControllerControllerTest2WayCommunication : CommandController
    {
        public static byte[] PayloadValue = { 0x21 };

        public CommandControllerControllerTest2WayCommunication() : base(PayloadValue)
        {
        }

        public override I2CCommandCode GetI2CCommandCode => I2CCommandCode.DeviceConverterCommand;
    }
}