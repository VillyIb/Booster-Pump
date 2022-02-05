using eu.iamia.i2c.communication.contract;

namespace BoosterPumpLibrary.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using eu.iamia.Util.Extensions;

    public class Register : IRegister
    {
        /// <summary>
        /// Maximum number of bytes matching in register.
        /// </summary>
        public const ushort MaxSize = 8;

        internal static void CheckRange(ushort value, ushort minValue, ushort maxValue, string name)
        {
            if (value.IsOutsideRange(minValue, maxValue))
            {
                throw new ArgumentOutOfRangeException(name, value, $"Range: {minValue}..{maxValue}");
            }
        }

        #region properties

        /// <summary>
        /// Information.
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Indicate the Register is read from an I2C Device
        /// </summary>
        public bool IsInput { get; }

        /// <summary>
        /// Indicate input data is valid.
        /// </summary>
        public bool IsInputDirty { get; set; }

        /// <summary>
        /// Indicate this Register is written to an I2C device.
        /// </summary>
        public bool IsOutput { get; }

        /// <summary>
        /// Indicate output data is valid.
        /// </summary>
        public bool IsOutputDirty { get; set; }

        /// <summary>
        /// Number of bytes this Register is managing {1..8}
        /// </summary>
        public ushort Size { get; protected set; }

        public byte RegisterAddress { get; protected internal set; }

        protected Dictionary<string, IBitSetting> SubRegisters { get; }


        #endregion

        protected Register()
        {
            SubRegisters = new();
        }

        /// <summary>
        /// Defines a subview <em>numberOfBits</em> bits long shifted <em>offsetInBits</em> bits with <em>description</em> as name.
        /// </summary>
        /// <param name="numberOfBits"></param>
        /// <param name="offsetInBits"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IBitSetting GetOrCreateSubRegister(ushort numberOfBits, ushort offsetInBits, string description = "")
        {
            var key = $"{offsetInBits}_{numberOfBits}_{description}";

            if (SubRegisters.ContainsKey(key))
            {
                return SubRegisters[key];
            }

            var max = Size * 8;

            if (numberOfBits + offsetInBits > max)
            {
                throw new ArgumentOutOfRangeException($"Size + offeset must be less or equal to {max}.");
            }

            var result = new BitSetting(numberOfBits, offsetInBits, this, description);
            SubRegisters.Add(key, result);
            return result;
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach (var current in SubRegisters.Values)
            {
                if (result.Length > 0)
                {
                    result.Append("\r\n");

                }
                result.AppendFormat(CultureInfo.InvariantCulture, $"{current.Description}: {current.MaskAsBinary()}, {current.Value} / 0x{current.Value:X8}, ");
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns Value as byte list of length ByteCount.
        /// Clears IsOutputDirty
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> GetByteValuesToWriteToDevice()
        {
            IsOutputDirty = false;
            var value = Value;
            var bytes = BitConverter.GetBytes(value);
            var reverse = bytes.Reverse().ToArray();
            var result = reverse.Skip(MaxSize - Size).Take(Size).ToArray();
            return result;
        }

        protected ulong ValueField;

        public ulong Value
        {
            get => ValueField;
            set
            {
                ValueField = value;
                IsOutputDirty = true; // TODO evaluate.
                IsInputDirty = false;
            }
        }

        public Register(byte registerAddress, string description, ushort sizeInBytes, Direction direction) : this()
        {
            CheckRange(registerAddress, 0, 127, nameof(registerAddress));
            CheckRange(sizeInBytes, 1, MaxSize, nameof(sizeInBytes));

            RegisterAddress = registerAddress;
            Description = description;
            Size = sizeInBytes;

            IsInput = (direction & Direction.Input) == Direction.Input;
            IsOutput = (direction & Direction.Output) == Direction.Output;
        }
    }
}
