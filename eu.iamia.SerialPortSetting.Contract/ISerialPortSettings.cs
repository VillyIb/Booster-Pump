﻿namespace eu.iamia.SerialPortSetting.Contract
{
    public interface ISerialPortSettings
    {
        string PortName { get; set; }

        int BaudRate { get; set; }

        int Timeout { get; set; }
    }
}