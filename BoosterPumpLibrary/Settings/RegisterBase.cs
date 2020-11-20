﻿using System;
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

        public byte RegisterIndex { get; protected set; }

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

        public BitSetting GetOrCreateSubRegister(int size, int offsett, string description = "")
        {
            var key = $"{offsett}_{size}_{description}";

            if (BitSettings.ContainsKey(key))
            {
                return BitSettings[key];
            }

            var max = Size * 8;

            if (size < 0 || offsett < 0 || size + offsett > max) { throw new ArgumentOutOfRangeException($"Size + offeset must be less or equal to {max}."); }

            var result = new BitSetting(size, offsett, description)
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
            var bytes = BitConverter.GetBytes(GetValue());
            var reverse = bytes.Reverse();
            var result = reverse.Skip(MaxSize - Size).Take(Size);
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

        protected RegisterBase(byte registerIndex, string description, int byteCount)
        {
            CheckRange((ushort)registerIndex, 0, 127, nameof(registerIndex));
            // ReSharper disable once VirtualMemberCallInConstructor
            CheckRange((ushort)byteCount, 1, MaxSize, nameof(byteCount));

            RegisterIndex = registerIndex;
            Description = description;
            Size = byteCount;
            Value = default;
        }      
    }
}