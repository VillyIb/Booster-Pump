#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Settings;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;

namespace Modules
{
    public abstract class InputModule : BaseModuleV2
    {
        protected InputModule(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        { }

        //public abstract void ReadFromDevice();

        public abstract bool IsInputValid { get; }

        public  ReadModuleEnumerator GetReadEnumerator()
        {
            var registersToSend = Registers.Where(t => t.IsInputDirty);
            return new ReadModuleEnumerator(registersToSend, DeviceAddress);
        }

        public virtual void ReadFromDevice()
        {
            using ReadModuleEnumerator enumerator = GetReadEnumerator();
            var currentRetryCount = RetryCount;
            while (enumerator.MoveNext() && enumerator.Current != null)
            {
                ICommand command = new CommandRead(enumerator.Current.RegisterAddress, (byte)enumerator.Current.Size);

                var fromDevice = ApiToSerialBridge.Execute(command);

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (
                    currentRetryCount <= 0
                    ||
                    fromDevice.Payload.Count == 1
                    &&
                    fromDevice.Payload[0] == ResponseWriteSuccess)
                {
                    enumerator.Current.Value = fromDevice.Value;
                    continue;
                }
                enumerator.Reset();
                currentRetryCount--;
            }
        }
    }

    public class ReadModuleEnumerator : IEnumerator<Register?>
    {
        protected readonly byte DeviceAddress;

        protected List<Register> SelectedRegisters { get; }

        public void Reset()
        {
            foreach (var current in SelectedRegisters)
            {
                current.SetInputDirty();
            }
        }

        [ExcludeFromCodeCoverage] 
        object? IEnumerator.Current => Current;

        
        /// <summary>
        /// CommandWrite or CommandRead
        /// </summary>
        public Register? Current { get; set; }

        public Register? CurrentReadCommand => (Register?)Current;

        public ReadModuleEnumerator(IEnumerable<Register> selectedRegisters, byte deviceAddress)
        {
            DeviceAddress = deviceAddress;
            SelectedRegisters = selectedRegisters.ToList();
            Current = null;
        }

        public  bool MoveNext()
        {
            Current = null;
            if (!SelectedRegisters.Any(t => t.IsInputDirty)) { return false; }

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
