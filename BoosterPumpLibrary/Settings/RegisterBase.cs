using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BoosterPumpLibrary.Settings
{
    public abstract class RegisterBase
    {
        internal abstract void SetValue(ulong value);

        internal abstract ulong GetValue(); // TODO Could be public

        /// <summary>
        /// Number of bytes matching T.
        /// </summary>
        protected abstract int MaxSize { get; }

        protected void CheckRange(dynamic value, dynamic minValue, dynamic maxValue, string name)
        {
            if (value < minValue || maxValue < value)
            {
                throw new ArgumentOutOfRangeException(name, value, $"Range: {minValue}..{maxValue}");
            }
        }

        /// <summary>
        /// Number of bytes this Register is managing {1..8}
        /// </summary>
        public int Size { get; protected set; }

        public string Description { get; protected set; }

        public byte RegisterAddress { get; protected set; }

        public bool IsDirty { get; protected set; }       

        protected Dictionary<string, BitSetting> BitSettings { get; }

        protected RegisterBase()
        {
            BitSettings = new Dictionary<string, BitSetting>();
        }

        public void SetDirty()
        {
            IsDirty = true;
        }

        public BitSetting GetOrCreateSubRegister(int size, int offset, string description = "")
        {
            var key = $"{offset}_{size}_{description}";

            if (BitSettings.ContainsKey(key))
            {
                return BitSettings[key];
            }

            var max = Size * 8;

            if (size < 0 || offset < 0 || size + offset > max) { throw new ArgumentOutOfRangeException($"Size + offeset must be less or equal to {max}."); }

            var result = new BitSetting(size, offset, description)
            {
                ParentRegister = this
            };
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

            foreach(var current in BitSettings.Values)
            {
                result.AppendFormat($"{current.Description}: {current.MaskAsBinary()}, ");
            }

            return result.ToString();
        }
    }

    public abstract class RegisterBase<T> : RegisterBase where T : struct
    {
        private T ValueField;

        public T Value { 
            get => ValueField; 
            set { 
                ValueField = value;
                IsDirty = true;
            } 
        }

        protected RegisterBase(byte registerAddress, string description, int byteCount)
        {
            CheckRange((ushort)registerAddress, 0, 127, nameof(registerAddress));
            // ReSharper disable once VirtualMemberCallInConstructor
            CheckRange((ushort)byteCount, 1, MaxSize, nameof(byteCount));

            RegisterAddress = registerAddress;
            Description = description;
            Size = byteCount;
        }      
    }
}
