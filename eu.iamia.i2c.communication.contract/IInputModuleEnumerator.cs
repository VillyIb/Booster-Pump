using System;
using eu.iamia.BaseModule.Contract;

namespace eu.iamia.i2c.communication.contract;

public interface IInputModuleEnumerator : IDisposable
{
    void Reset();

    /// <summary>
    /// CommandWrite or CommandRead
    /// </summary>
    IRegister? Current { get; set; }

    bool MoveNext();
}