#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BoosterPumpLibrary.Settings;
using eu.iamia.NCD.API;

namespace BoosterPumpLibrary.ModuleBase
{
    public abstract partial class BaseModuleV2
    {
        public class ModuleEnumerator : IEnumerator<CommandWrite?>
        {
            private readonly byte DeviceAddress;

            private List<RegisterBase> SelectedRegisters { get; }

            public CommandWrite? Current { get; set; }

            object? IEnumerator.Current => Current;

            public ModuleEnumerator(IEnumerable<RegisterBase> selectedRegisters, byte deviceAddress)
            {
                DeviceAddress = deviceAddress;
                SelectedRegisters = selectedRegisters.ToList();
                Current = null;
            }

            public void Reset()
            {
                foreach (var current in SelectedRegisters)
                {
                    current.SetOutputDirty();
                }
            }

            public bool MoveNext()
            {
                Current = null;
                if (!SelectedRegisters.Any(t => t.IsOutputDirty)) { return false; }

                var currentCommand = new List<byte>();
                byte currentRegisterAddress = 0;

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var current in SelectedRegisters)
                {
                    if (!current.IsOutputDirty) continue;

                    if (currentCommand.Count > 0 && currentRegisterAddress + 1 != current.RegisterAddress) { break; }
                    if (currentCommand.Count == 0)
                    {
                        currentCommand.Add(current.RegisterAddress);
                    }
                    currentCommand.AddRange(current.GetByteValuesToWriteToDevice());
                    currentRegisterAddress = current.RegisterAddress;
                }

                var writeCommand = new CommandWrite(DeviceAddress, currentCommand);

                Current = writeCommand;

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
}