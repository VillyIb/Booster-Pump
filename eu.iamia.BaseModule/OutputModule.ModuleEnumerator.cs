#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using eu.iamia.BaseModule.Contract;
using eu.iamia.i2c.communication.contract;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;

namespace eu.iamia.BaseModule
{
    public partial class OutputModule
    {
        public class OutputModuleEnumerator : IEnumerator<ICommand?>, IOutputModuleEnumerator
        {
            protected readonly byte DeviceAddress;

            protected List<IRegister> SelectedRegisters { get; }

            /// <summary>
            /// CommandWrite or CommandRead
            /// </summary>
            public ICommand? Current { get; set; }

            [ExcludeFromCodeCoverage]
            object? IEnumerator.Current => Current;

            public ICommand? CurrentWriteCommand => (CommandWrite?)Current;

            public OutputModuleEnumerator(IEnumerable<IRegister> selectedRegisters, byte deviceAddress)
            {
                DeviceAddress = deviceAddress;
                SelectedRegisters = selectedRegisters.ToList();
                Current = null;
            }

            public void Reset()
            {
                foreach (var current in SelectedRegisters)
                {
                    current.IsOutputDirty = true;
                }
            }

            public virtual bool MoveNext()
            {
                Current = null;
                if (!SelectedRegisters.Any(register => register.IsOutput && register.IsOutputDirty)) { return false; }

                var currentCommand = new List<byte>();
                byte currentRegisterAddress = 0;

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var current in SelectedRegisters)
                {
                    if (!current.IsOutput) continue;
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