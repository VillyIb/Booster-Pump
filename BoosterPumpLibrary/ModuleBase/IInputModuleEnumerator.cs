using System;
using BoosterPumpLibrary.Settings;

namespace BoosterPumpLibrary.ModuleBase;

public interface IInputModuleEnumerator : IDisposable
{
    void Reset();

    /// <summary>
    /// CommandWrite or CommandRead
    /// </summary>
    IRegister? Current { get; set; }

    bool MoveNext();
}