﻿using System;
using eu.iamia.NCD.API.Contract;

namespace eu.iamia.i2c.communication.contract;

public interface IOutputModuleEnumerator : IDisposable
{
    /// <summary>
    /// CommandWrite or CommandRead
    /// </summary>
    ICommand? Current { get; set; }

    ICommand? CurrentWriteCommand { get; }

    void Reset();

    bool MoveNext();
}