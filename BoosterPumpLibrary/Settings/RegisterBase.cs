using System;
using System.Collections.Generic;

namespace BoosterPumpLibrary.Settings
{
    public abstract class RegisterBase
    {
        internal abstract void SetValue(ulong value);

        internal abstract ulong GetValue();

        /// <summary>
        /// Number of bytes in this setting, range: 1..8.
        /// </summary>
        public int ByteCount { get; protected set; }

        public string Description { get; protected set; }

        public byte RegisterIndex { get; protected set; }

        public bool IsDirty { get; protected set; }
    }


    public abstract class RegisterBase<T> : RegisterBase where T : struct
    {
        private T ValueField;
        public T Value { 
            get => ValueField; 
            protected set { 
                ValueField = value;
                IsDirty = true;
            } 
        }

        protected void CheckRange(dynamic value, dynamic minValue, dynamic maxValue, string name)
        {
            if (value < minValue || maxValue < value)
            {
                throw new ArgumentOutOfRangeException(name, value, $"Range: {minValue}..{maxValue}");
            }
        }

        /// <summary>
        /// Number of bytes matching T.
        /// </summary>
        protected abstract int MaxByteSize { get; }

        protected List<BitSetting> BitSettings { get; }

        public BitSetting CreateSubRegister(int size, int offsett, string description = "")
        {
            var max = ByteCount * 8;

            if (size < 0 || offsett < 0 || (size + offsett) > max) { throw new ArgumentOutOfRangeException($"Size + offeset must be less or equal to {max}."); }

            var result = new BitSetting(size, offsett, description)
            {
                ParentRegister = this
            };
            BitSettings.Add(result);
            return result;
        }

        public RegisterBase(byte registerIndex, string description, int byteCount)
        {
            CheckRange((ushort)registerIndex, 0, 127, nameof(registerIndex));
            CheckRange((ushort)byteCount, 1, MaxByteSize, nameof(byteCount));

            RegisterIndex = registerIndex;
            Description = description;
            BitSettings = new List<BitSetting>();
            ByteCount = byteCount;
            Value = default;
        }

        public void SetDirty()
        {
            IsDirty = true;
        }
    }
}
