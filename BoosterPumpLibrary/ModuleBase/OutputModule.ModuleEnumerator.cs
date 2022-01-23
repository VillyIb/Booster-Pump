#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using BoosterPumpLibrary.Settings;
using eu.iamia.NCD.API;

namespace BoosterPumpLibrary.ModuleBase
{
    public abstract partial class BaseModuleV2
    {
        public class WriteModuleEnumerator : IEnumerator<CommandDevice?>
        {
            protected readonly byte DeviceAddress;

            protected List<Register> SelectedRegisters { get; }

            /// <summary>
            /// CommandWrite or CommandRead
            /// </summary>
            public CommandDevice? Current { get; set; }

            [ExcludeFromCodeCoverage]
            object? IEnumerator.Current => Current;

            public CommandWrite? CurrentWriteCommand => (CommandWrite?)Current;

            public WriteModuleEnumerator(IEnumerable<Register> selectedRegisters, byte deviceAddress)
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

            public virtual bool MoveNext()
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