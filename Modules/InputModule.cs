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
    public abstract class InputModule : OutputModule
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
            using var register = GetReadEnumerator();
            var currentRetryCount = RetryCount;
            while (register.MoveNext() && register.Current != null && currentRetryCount > 0)
            {
                var lengthRequested = (byte)register.Current.Size;
                ICommand command = new CommandRead(register.Current.RegisterAddress, lengthRequested);

                var response = ApiToSerialBridge.Execute(command);

                if (
                    response.IsValid 
                    &&  
                    !response.IsError 
                    && 
                    response.ByteCount == lengthRequested
                )
                {
                    register.Current.Value = response.Value;
                    continue;
                }
                register.Reset();
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
