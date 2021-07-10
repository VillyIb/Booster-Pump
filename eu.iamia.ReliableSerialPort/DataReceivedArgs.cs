using System;

namespace eu.iamia.ReliableSerialPort
{
    public class DataReceivedArgs : EventArgs
    {
        public byte[] Data { get; set; }
    }
}