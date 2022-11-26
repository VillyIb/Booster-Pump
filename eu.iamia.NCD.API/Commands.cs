using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using EnsureThat;
using eu.iamia.NCD.API.Contract;
// ReSharper disable UnusedMember.Global

namespace eu.iamia.NCD.API
{
    // see:https://ncd.io/serial-to-i2c-conversion/

    // TODO Refactor to let Command implement Execute method calling Gateway.Execute(command) and verify the response is valid eg. requested length is OK

    public abstract class Command : ICommand
    {
        public abstract IEnumerable<byte> I2C_Data();

        public abstract I2CDeviceOperation GetI2CDeviceOperation { get; }

        public byte LengthRequested { get; init; }

        protected Command(byte lengthRequested)
        {
            LengthRequested = lengthRequested;
        }

        // ReSharper disable once UnusedMember.Global
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

        protected CommandDevice(byte deviceAddress, byte lengthRequested) : base(lengthRequested)
        {
            DeviceAddress = deviceAddress;
        }
    }

    public class CommandRead : CommandDevice
    {
        public CommandRead(byte deviceAddress, byte lengthRequested)
            : base(deviceAddress, lengthRequested)
        { }

        public override IEnumerable<byte> I2C_Data()
        {
            yield return DeviceAddress;
            yield return LengthRequested;
        }

        public override I2CDeviceOperation GetI2CDeviceOperation => I2CDeviceOperation.DeviceRead;
    }

    public class CommandWrite : CommandDevice
    {
        public List<byte> Payload { get; set; }

        public CommandWrite(byte deviceAddress, IEnumerable<byte> payload)
            : base(deviceAddress, 1)
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

        public override I2CDeviceOperation GetI2CDeviceOperation => I2CDeviceOperation.DeviceWrite;
    }

    public class CommandWriteRead : CommandDevice
    {
        public List<byte> Payload { get; }

        public byte Delay { get;  }

        public CommandWriteRead(byte deviceAddress, IEnumerable<byte> payload, byte lengthRequested, byte delay = 0x16)
            : base(deviceAddress, lengthRequested)
        {
            Ensure.That(payload, nameof(payload)).IsNotNull();
            Payload = payload.ToList();
            Ensure.That(Payload, nameof(payload)).SizeIs(Math.Min(255, Payload.Count));
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

        public override I2CDeviceOperation GetI2CDeviceOperation => I2CDeviceOperation.DeviceWriteRead;
    }

    public abstract class CommandController : Command
    {
        private IImmutableList<byte> Payload { get; }

        protected CommandController(IEnumerable<byte> payload) : base(0)
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

        public override I2CDeviceOperation GetI2CDeviceOperation => I2CDeviceOperation.DeviceBusScan;
    }

    public class CommandControllerControllerStop : CommandController
    {
        public static byte[] PayloadValue = { 0x21, 0xBB };

        public CommandControllerControllerStop() : base(PayloadValue)
        {
        }

        public override I2CDeviceOperation GetI2CDeviceOperation => I2CDeviceOperation.DeviceConverterCommand;
    }

    public class CommandControllerControllerReboot : CommandController
    {
        public static byte[] PayloadValue = { 0x21, 0xBC };

        public CommandControllerControllerReboot() : base(PayloadValue)
        {
        }

        public override I2CDeviceOperation GetI2CDeviceOperation => I2CDeviceOperation.DeviceConverterCommand;
    }

    public class CommandControllerControllerHardReboot : CommandController
    {
        public static byte[] PayloadValue = { 0x21, 0xBD };

        public CommandControllerControllerHardReboot() : base(PayloadValue)
        {
        }

        public override I2CDeviceOperation GetI2CDeviceOperation => I2CDeviceOperation.DeviceConverterCommand;
    }

    public class CommandControllerControllerTest2WayCommunication : CommandController
    {
        public static byte[] PayloadValue = { 0x21 };

        public CommandControllerControllerTest2WayCommunication() : base(PayloadValue)
        {
        }

        public override I2CDeviceOperation GetI2CDeviceOperation => I2CDeviceOperation.DeviceConverterCommand;
    }
}