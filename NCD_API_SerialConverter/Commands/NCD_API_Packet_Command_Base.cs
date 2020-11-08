namespace SerialConverter.Commands
{
    using System.Collections.Generic;
    using System.Text;
    using BoosterPumpLibrary.Commands;
    using NCD_API_SerialConverter.Contracts;

    public abstract class NCD_API_Packet_Command_Base<T> : IX where T : CommandBase
    {
        public T BackingValue { get; }

        public NCD_API_Packet_Command_Base(T backingField)
        {
            BackingValue = backingField;
        }

        public NCD_API_Packet_Command_Base()
        {
            BackingValue = null;
        }

        public virtual byte Header => 0xAA;

        public abstract byte Length { get; }

        public abstract byte Command { get; }

        public virtual byte? Address { get => BackingValue?.Address; }

        public byte Checksum
        {
            get
            {
                var checksum = Header + Length;
                foreach (var current in CommandData())
                {
                    checksum += current;
                }
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
