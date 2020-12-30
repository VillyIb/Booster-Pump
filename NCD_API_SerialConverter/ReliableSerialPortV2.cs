using System;
using System.IO.Ports;

namespace NCD_API_SerialConverter
{
    public class ReliableSerialPortV2 : SerialPort
    {
        public ReliableSerialPortV2(string portName, int baudRate)
        {
            PortName = portName;
            BaudRate = baudRate;
            //DataBits = dataBits;
            //Parity = parity;
            //StopBits = stopBits;
            Handshake = Handshake.None;
            DtrEnable = true;
            NewLine = Environment.NewLine;
            ReceivedBytesThreshold = 1024;
        }

        public new void Open()
        {
            base.Open();
            ContinuousRead();
        }

        public event EventHandler<DataReceivedArgs> DataReceived;

        public virtual void OnDataReceived(byte[] data)
        {
            var handler = DataReceived;
            if (handler != null)
            {
                handler(this, new DataReceivedArgs { Data = data });
            }
        }

        private void ContinuousRead()
        {
            const int buffersize = 4096;
            byte[] buffer = new byte[buffersize];

            void KickoffRead() =>
                BaseStream.BeginRead(buffer, 0, buffersize, delegate(IAsyncResult ar)
                {
                    int count = base.BaseStream.EndRead(ar);
                    byte[] dst = new byte[count];
                    Buffer.BlockCopy(buffer, 0, dst, 0, count);
                    OnDataReceived(dst);
                    KickoffRead();
                }, null);

            KickoffRead();
        }

    }

    public class DataReceivedArgs : EventArgs
    {
        public byte[] Data { get; set; }
    }
}
