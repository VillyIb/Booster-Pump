using BoosterPumpLibrary.ModuleBase;

namespace NCD_API_SerialConverter.NcdApiProtocol.DeviceCommands
{
    using System.Collections.Generic;
    using BoosterPumpLibrary.Commands;
    using NcdApiProtocol;

    public class DeviceRead : DeviceBase<ReadCommand>, IDevice
    {
        public DeviceRead(ReadCommand backingValue) : base(backingValue)
        { }

        public override byte Length => 0x03;

        public override byte Command => 0xBF;

        public byte LengthRequested => BackingValue.LengthRequested;

        public override IEnumerable<byte> CommandData()
        {
            yield return Command;
            yield return Address ?? 0x00;
            yield return LengthRequested;
        }
    }
}
