using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using eu.iamia.NCD.DeviceCommunication.Contract;

namespace eu.iamia.NCD.API
{
    // see:https://ncd.io/serial-to-i2c-conversion/

    public abstract class Command : ICommand
    {
        public abstract IEnumerable<byte> I2C_Data();

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

    public abstract class CommandDevice : Command, ICommandDevice
    {
        public byte DeviceAddress { get; set; }

        protected CommandDevice(byte deviceAddress)
        {
            DeviceAddress = deviceAddress;
        }
    }

    public class CommandRead : CommandDevice, ICommandRead
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
    }

    public class CommandWrite : CommandDevice, ICommandWrite
    {
        public IEnumerable<byte> Payload { get; set; }

        public CommandWrite(byte deviceAddress, IEnumerable<byte> payload)
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

    public class CommandWriteRead : CommandWrite, ICommandWriteRead
    {
        public byte LengthRequested { get; set; }

        public byte Delay { get; set; }

        public CommandWriteRead(byte deviceAddress, IEnumerable<byte> payload, byte lengthRequested, byte delay = 0x16)
            : base(deviceAddress, payload)
        {
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
    }
    

    public abstract class CommandController : Command
    {
        private IImmutableList<byte> Payload { get; }

        protected CommandController(IEnumerable<byte> payload) : base()
        {
            Payload = ImmutableList<byte>.Empty.AddRange(payload);
        }

        public override IEnumerable<byte> I2C_Data()
        {
            foreach (var value in Payload)
            {
                yield return value;
            }
        }
    }

    public class CommandControllerControllerBusSCan : CommandController, ICommandControllerBusScan
    {
        public static byte[] PayloadValue = new byte[] {0x00};

        public CommandControllerControllerBusSCan() : base( PayloadValue )
        { }
    }

    public class CommandControllerControllerStop : CommandController, ICommandControllerStop
    {
        public static byte[] PayloadValue = new byte[] { 0x21, 0xBB };

        public CommandControllerControllerStop() : base(PayloadValue)
        { }
    }

    public class CommandControllerControllerReboot : CommandController, ICommandControllerReboot
    {
        public static byte[] PayloadValue = new byte[] { 0x21, 0xBC };
        
        public CommandControllerControllerReboot() : base(PayloadValue)
        { }
    }

    public class CommandControllerControllerHardReboot : CommandController, ICommandControllerHardReboot
    {
        public static byte[] PayloadValue = new byte[] { 0x21, 0xBD };
        
        public CommandControllerControllerHardReboot() : base(PayloadValue )
        { }
    }

    public class CommandControllerControllerTest2WayCommunication : CommandController, ICommandControllerTest2WayCommunication
    {
        public static byte[] PayloadValue = new byte[] { 0x21 };
        
        public CommandControllerControllerTest2WayCommunication() : base(PayloadValue)
        { }
    }

}
