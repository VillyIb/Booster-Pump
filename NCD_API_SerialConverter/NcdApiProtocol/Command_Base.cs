using System.Linq;

namespace NCD_API_SerialConverter.NcdApiProtocol
{
    using System.Collections.Generic;
    using System.Text;
    using BoosterPumpLibrary.Commands;
    using Contracts;

    public abstract class CommandBase<T> : IX where T : CommandBase
    {
        public T BackingValue { get; }

        protected CommandBase(T backingField)
        {
            BackingValue = backingField;
        }

        // ReSharper disable once UnusedMember.Global
        protected CommandBase()
        {
            BackingValue = null;
        }

        public virtual byte Header => 0xAA;

        public abstract byte Length { get; }

        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract byte Command { get; }

        public virtual byte? Address { get => BackingValue?.DeviceAddress; }

        public byte Checksum
        {
            get
            {
                var checksum = Header + Length;
                checksum = CommandData().Aggregate(checksum, (current1, current) => current1 + current);
                return (byte)(checksum & 0xff);
            }
        }

        public abstract IEnumerable<byte> CommandData();


        public IEnumerable<byte> ApiEncodedData()
        {
            yield return Header;
            yield return Length;
            foreach (var current in CommandData())
            {
                yield return current;
            }
            yield return Checksum;
        }

        // ReSharper disable once UnusedMember.Global
        public string CommandDataAsHex
        {
            get
            {
                var result = new StringBuilder();

                foreach (var current in CommandData())
                {
                    result.AppendFormat($"{current:X2} ");
                }

                return result.ToString();
            }
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach (var current in ApiEncodedData())
            {
                result.AppendFormat($"{current:X2} ");
            }

            return result.ToString();
        }
    }
}
