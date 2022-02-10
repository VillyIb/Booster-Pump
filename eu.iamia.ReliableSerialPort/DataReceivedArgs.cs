using System;
using eu.iamia.SerialPortSetting.Contract;

namespace eu.iamia.ReliableSerialPort
{
    public class DataReceivedArgs : EventArgs, IDataReceivedArgs
    {
        public byte[] Data { get; set; }
    }
}