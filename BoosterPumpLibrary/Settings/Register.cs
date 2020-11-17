using System;
using System.Linq;
using System.Collections.Generic;

namespace BoosterPumpLibrary.Settings
{

    public class Register
    {
        public ulong Value { get; protected set; }

        public byte RegisterIndex { get; protected set; }

        public bool IsDirty { get; protected set; }

        public IEnumerable<BitSetting> BitSettings { get; }

        public string Description { get; protected set; }

        /// <summary>
        /// Number of bytes in this setting, range: 1..8.
        /// </summary>
        public int ByteCount { get; protected set; }

        private Register()
        { }

        protected void CheckRange(ulong value, ulong minValue, ulong maxValue, string name)
        {
            if (value < minValue || maxValue < value)
            {
                throw new ArgumentOutOfRangeException(name, value, $"Range: {minValue}..{maxValue}");
            }
        }

        public Register(byte registerIndex, string description, int byteCount, params BitSetting[] bitSettings)
        {
            CheckRange(registerIndex, 0, 127, nameof(registerIndex));
            CheckRange((ulong)byteCount, 1, 8, nameof(byteCount));

            RegisterIndex = registerIndex;
            Description = description;
            BitSettings = bitSettings.ToList();
            ByteCount = byteCount;
            Value = 0;

            foreach (var bs in BitSettings)
            {
                bs.ParentRegister = this;
            }
        }

        public void SetDataRegister(ulong value)
        {
            var shift = 8 * ByteCount;
            CheckRange(value, 0UL, (1UL << shift) - 1, nameof(value));
            var max = ulong.MaxValue;
            if (max < value) { throw new ArgumentOutOfRangeException(nameof(value), value, "Valid: {0..2**32}"); }
            Value = value;
            IsDirty = true;
        }

        public void SetDirty()
        {
            IsDirty = true;
        }

        public ulong GetDataRegisterAndClearDirty()
        {
            IsDirty = false;
            return Value;
        }

    }
}
