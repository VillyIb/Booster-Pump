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

        public byte RegisterIndex { get; protected set; }

        public bool IsDirty { get; protected set; }       

        protected List<BitSetting> BitSettings { get; }

        protected RegisterBase()
        {
            BitSettings = new List<BitSetting>();
        }

        public void SetDirty()
        {
            IsDirty = true;
        }

        public BitSetting CreateSubRegister(int size, int offsett, string description = "")
        {
            var max = Size * 8;

            if (size < 0 || offsett < 0 || (size + offsett) > max) { throw new ArgumentOutOfRangeException($"Size + offeset must be less or equal to {max}."); }

            var result = new BitSetting(size, offsett, description)
            {
                ParentRegister = this
            };
            BitSettings.Add(result);
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
            return BitConverter.GetBytes(GetValue()).Skip(MaxSize - Size).Take(Size);
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach(var current in BitSettings)
            {
                result.AppendFormat($"{current.Description}: {BitSetting.ToBinary( current.Mask)}, ");
            }

            return result.ToString();
        }
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

    
        public RegisterBase(byte registerIndex, string description, int byteCount)
        {
            CheckRange((ushort)registerIndex, 0, 127, nameof(registerIndex));
            CheckRange((ushort)byteCount, 1, MaxSize, nameof(byteCount));

            RegisterIndex = registerIndex;
            Description = description;
            Size = byteCount;
            Value = default;
        }      
    }
}
