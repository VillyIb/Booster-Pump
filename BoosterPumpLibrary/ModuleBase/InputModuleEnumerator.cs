using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BoosterPumpLibrary.Settings;

namespace BoosterPumpLibrary.ModuleBase
{
    public class InputModuleEnumerator : IEnumerator<Register?>
    {
        protected readonly byte DeviceAddress;

        protected List<Register> SelectedRegisters { get; }

        public void Reset()
        {
            foreach (var current in SelectedRegisters)
            {
                current.IsInputDirty = true;
            }
        }

        [ExcludeFromCodeCoverage]
        object? IEnumerator.Current => Current;


        /// <summary>
        /// CommandWrite or CommandRead
        /// </summary>
        public Register? Current { get; set; }

        public Register? CurrentReadCommand => (Register?)Current;

        public InputModuleEnumerator(IEnumerable<Register> selectedRegisters, byte deviceAddress)
        {
            DeviceAddress = deviceAddress;
            SelectedRegisters = selectedRegisters.ToList();
            Current = null;
        }

        public bool MoveNext()
        {
            Current = null;
            if (!SelectedRegisters.Any(register => register.IsInput && register.IsInputDirty)) { return false; }

            Current = SelectedRegisters.First(current => current.IsInputDirty);
            return true;
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing) return;

            SelectedRegisters.Clear();
            Current = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}