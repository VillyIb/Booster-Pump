using System;
using eu.iamia.NCD.API;

namespace BoosterPumpLibrary.ModuleBase;

public interface IOutputModuleEnumerator : IDisposable
{
    /// <summary>
    /// CommandWrite or CommandRead
    /// </summary>
    CommandDevice? Current { get; set; }

    CommandWrite? CurrentWriteCommand { get; }

    void Reset();

    bool MoveNext();
}