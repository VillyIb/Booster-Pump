using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Settings;

namespace eu.iamia.BaseModule
{
    public class InputModuleEnumerator : IEnumerator<IRegister?>, IInputModuleEnumerator
    {
        protected readonly byte DeviceAddress;

        protected List<IRegister> SelectedRegisters { get; }

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
        public IRegister? Current { get; set; }

        public InputModuleEnumerator(IEnumerable<IRegister> selectedRegisters, byte deviceAddress)
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