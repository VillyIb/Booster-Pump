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
            DataReceived?.Invoke(this, new DataReceivedArgs { Data = data });
        }

        private const int BufferSize = 128;
        private readonly byte[] Buffer = new byte[BufferSize];

        private void KickoffRead() =>
            BaseStream.BeginRead(
                Buffer,
                0,
                BufferSize,
                delegate (IAsyncResult ar)
                {
                    try
                    {
                        var count = base.BaseStream.EndRead(ar); // InvalidOperationException if port is closed.
                        var dst = new byte[count];
                        System.Buffer.BlockCopy(Buffer, 0, dst, 0, count);
                        OnDataReceived(dst);
                        KickoffRead(); // loop after finished reading - pushing to stack?
                    }
                    catch (InvalidOperationException ex)
                    {
                        var msg = ex.Message;
                    }
                },
                null
            );

    

        private void ContinuousRead()
        {
            KickoffRead();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed state
                DataReceived = null;
            }
            base.Dispose(disposing);
        }
    }

    public class DataReceivedArgs : EventArgs
    {
        public byte[] Data { get; set; }
    }
}
