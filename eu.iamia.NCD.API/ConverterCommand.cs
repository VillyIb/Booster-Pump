using System.Collections.Generic;
using System.Collections.Immutable;
using eu.iamia.NCD.DeviceCommunication.Contract;

namespace eu.iamia.NCD.API
{
    public class ConverterCommand : CommandBase
    {
        private IImmutableList<byte> Payload { get; }

        public ConverterCommand(IEnumerable<byte> payload) : base(0)
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

    public class BusScanCommand : ConverterCommand, ICommandBusScan
    {
        public BusScanCommand() : base(new byte[] { 0x00 })
        { }
    }

    public class DeviceStop : ConverterCommand, ICommandStop
    {
        public DeviceStop() : base(new byte[] { 0x21, 0xBB })
        { }
    }

    public class DeviceRebootCommand : ConverterCommand, ICommandReboot
    {
        public DeviceRebootCommand() : base(new byte[] { 0x21, 0xBC })
        { }
    }

    public class DeviceHardRebootCommand : ConverterCommand, ICommandHardReboot
    {
        public DeviceHardRebootCommand() : base(new byte[] { 0x21, 0xBD })
        { }
    }

    public class DeviceTest2WayCommand : ConverterCommand, ICommandTest2WayCommunication
    {
        public DeviceTest2WayCommand() : base(new byte[] { 0x21 })
        { }
    }

}
