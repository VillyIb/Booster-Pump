using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using eu.iamia.Util.Extensions;

namespace BoosterPumpLibrary.Settings
{
    public abstract class RegisterBase
    {
        internal abstract void SetValue(ulong value);

        internal abstract ulong GetValue(); // TODO Could be public

        /// <summary>
        /// Number of bytes matching T.
        /// </summary>
        protected abstract ushort MaxSize { get; }

        protected void CheckRange(ushort value, ushort minValue, ushort maxValue, string name)
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

        public bool IsDirty { get; protected set; }

        protected Dictionary<string, BitSetting> BitSettings { get; }

        protected RegisterBase()
        {
            BitSettings = new();
        }

        public void SetDirty()
        {
            IsDirty = true;
        }

        public BitSetting GetOrCreateSubRegister(ushort size, ushort offset, string description = "")
        {
            var key = $"{offset}_{size}_{description}";

            if (BitSettings.ContainsKey(key))
            {
                return BitSettings[key];
            }

            var max = Size * 8;

            if (size + offset > max) { throw new ArgumentOutOfRangeException($"Size + offeset must be less or equal to {max}."); }

            var result = new BitSetting(size, offset, this, description);
            BitSettings.Add(key, result);
            return result;
        }

        /// <summary>
        /// Returns Value as byte list of length ByteCount.
        /// Clears IsDirty
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> GetByteValue()
        {
            IsDirty = false;
            var value = GetValue();
            var bytes = BitConverter.GetBytes(value);
            var reverse = bytes.Reverse().ToArray();
            var result = reverse.Skip(MaxSize - Size).Take(Size).ToArray();
            return result;
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach (var current in BitSettings.Values)
            {
                if (result.Length > 0)
                {
                    result.Append("\r\n");

                }
                result.AppendFormat($"{current.Description}: {current.MaskAsBinary()}, 0x{current.Value:X8}, ");
            }

            return result.ToString();
        }
    }

    public abstract class RegisterBase<T> : RegisterBase where T : struct
    {
        private T ValueField;

        public T Value
        {
            get => ValueField;
            set
            {
                ValueField = value;
                IsDirty = true;
            }
        }

        protected RegisterBase(byte registerAddress, string description, ushort byteCount)
        {
            CheckRange((ushort)registerAddress, (ushort)0, (ushort)127, nameof(registerAddress));
            // ReSharper disable once VirtualMemberCallInConstructor
            CheckRange(byteCount, 1, MaxSize, nameof(byteCount));

            RegisterAddress = registerAddress;
            Description = description;
            Size = byteCount;
        }
    }
}
