using System;

namespace eu.iamia.SerialPortSetting.Contract
{
    public interface IBufferLine
    {
        DateTime Timestamp { get; }

        string LogText { get; }
    }
}