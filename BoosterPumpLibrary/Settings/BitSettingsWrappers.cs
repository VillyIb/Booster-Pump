using System;
using System.Linq;
using eu.iamia.i2c.communication.contract;
using eu.iamia.Util.Extensions;

namespace BoosterPumpLibrary.Settings
{
    public abstract class BitSettingsWrapperBase
    {
        protected readonly IBitSetting Server;

        protected BitSettingsWrapperBase(IBitSetting server)
        {
            Server = server;
        }
    }

    public class Int8BitSettingsWrapper : BitSettingsWrapperBase
    {
        public Int8BitSettingsWrapper(IBitSetting server) : base(server)
        { }

        public sbyte Value
        {
            get => ((byte)Server.Value).ToInt8();
            set => Server.Value = value.ToUInt8();
        }
    }

    public class UInt8BitSettingsWrapper : BitSettingsWrapperBase
    {
        public UInt8BitSettingsWrapper(IBitSetting server) : base(server)
        { }

        public byte Value
        {
            get => (byte)Server.Value;
            set => Server.Value = value;
        }
    }

    public class Int16BitSettingsWrapper : BitSettingsWrapperBase
    {
        public Int16BitSettingsWrapper(IBitSetting server) : base(server)
        { }

        public short Value
        {
            get => ((ushort)Server.Value).ToInt16();
            set => Server.Value = value.ToUInt16();
        }
    }

    public class Int24BitSettingsWrapper : BitSettingsWrapperBase
    {
        public Int24BitSettingsWrapper(IBitSetting server) : base(server)
        { }

        public int Value
        {
            get => ((uint)Server.Value).ToInt24();
            set => Server.Value = value.ToUint24();
        }
    }

    public class EnumBitSettings<T> : BitSettingsWrapperBase where T : Enum
    {
        private readonly T[] Enums;

        public EnumBitSettings(IBitSetting server) : base(server)
        {
            Enums = (T[])Enum.GetValues(typeof(T));
        }

        public T? Value
        {
            get => Enums.FirstOrDefault(current => Convert.ToUInt64(current) == Server.Value);

            set => Server.Value = Convert.ToUInt64(value);
        }
    }
}
