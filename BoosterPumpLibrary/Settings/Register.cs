namespace BoosterPumpLibrary.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using eu.iamia.Util.Extensions;

    public class Register
    {
        /// <summary>
        /// Maximum number of bytes matching in register.
        /// </summary>
        protected static ushort MaxSize => 8;

        internal static void CheckRange(ushort value, ushort minValue, ushort maxValue, string name)
        {
            if (value.IsOutsideRange(minValue, maxValue))
            {
                throw new ArgumentOutOfRangeException(name, value, $"Range: {minValue}..{maxValue}");
            }
        }

        /// <summary>
        /// Number of bytes this Register is managing {1..8}
        /// </summary>
        public ushort Size { get; protected set; }

        public string Description { get; protected set; }

        public byte RegisterAddress { get; protected set; }

        protected Dictionary<string, BitSetting> SubRegisters { get; }

        protected bool IsOutputDirtyField;

        protected bool IsInputDirtyField;

        /// <summary>
        /// Indicate input data is valid.
        /// </summary>
        public bool IsOutputDirty => IsOutputDirtyField;

        public bool IsInputDirty => IsInputDirtyField;

        public void SetOutputDirty()
        {
            IsOutputDirtyField = true;
        }

        public void SetInputDirty()
        {
            IsInputDirtyField = true;
        }

        protected Register()
        {
            SubRegisters = new();
        }

        /// <summary>
        /// Defines a subview <em>size</em> bits long shifted <em>offset</em> bits with <em>description</em> as name.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="offset"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public BitSetting GetOrCreateSubRegister(ushort size, ushort offset, string description = "")
        {
            var key = $"{offset}_{size}_{description}";

            if (SubRegisters.ContainsKey(key))
            {
                return SubRegisters[key];
            }

            var max = Size * 8;

            if (size + offset > max)
            {
                throw new ArgumentOutOfRangeException($"Size + offeset must be less or equal to {max}.");
            }

            var result = new BitSetting(size, offset, this, description);
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
            IsOutputDirtyField = false;
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
                IsOutputDirtyField = true;
                IsInputDirtyField = false;
            }
        }

        public Register(byte registerAddress, string description, ushort byteCount) : this()
        {
            CheckRange(registerAddress, 0, 127, nameof(registerAddress));
            CheckRange(byteCount, 1, MaxSize, nameof(byteCount));

            RegisterAddress = registerAddress;
            Description = description;
            Size = byteCount;
        }
    }
}
