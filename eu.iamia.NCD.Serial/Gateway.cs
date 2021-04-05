using System;
using System.Collections.Generic;
using System.Threading;
using eu.iamia.NCD.Serial.Contract;
using eu.iamia.ReliableSerialPort;
using eu.iamia.Util;

namespace eu.iamia.NCD.Serial
{
    public class Gateway : IGateway, IDisposable
    {
        private static TimeSpan ReadTimeout => TimeSpan.FromSeconds(5);

        private ISerialPortDecorator SerialPort { get; }

        public Gateway(ISerialPortDecorator serialPort)
        {
            SerialPort = serialPort;
        }

        private readonly AutoResetEvent ResultReady = new AutoResetEvent(false);

        private NcdState State;

        private List<byte> Payload;

        private static byte Header => 0xAA;

        private byte ByteCount { get; set; }

        private byte Checksum { get; set; }

        private void ProcessInput(object sender, DataReceivedArgs args)
        {
            foreach (var current in args.Data)
            {
                switch (State)
                {
                    case NcdState.ExpectHeader:
                        {
                            if (Header == current)
                            {
                                State = NcdState.ExpectLength;
                            }
                            break;
                        }

                    case NcdState.ExpectLength:
                        {
                            ByteCount = current;
                            Payload = new List<byte>(ByteCount);
                            State = NcdState.ExpectPayload;
                            break;
                        }

                    case NcdState.ExpectPayload:
                        {
                            Payload.Add(current);
                            if (ByteCount <= Payload.Count)
                            {
                                State = NcdState.ExpectChecksum;
                            }
                            break;
                        }

                    case NcdState.ExpectChecksum:
                        {
                            Checksum = current;
                            ResultReady.Set();
                            State = NcdState.Overflow;
                            break;
                        }

                    case NcdState.Overflow:
                        break;

                    default:
                        break;
                }
            }
        }

        private bool IsInitialized;

        private void Init()
        {
            State = NcdState.ExpectHeader;
            ByteCount = byte.MinValue;
            Payload = null;
            Checksum = byte.MinValue;

            if (IsInitialized) return;

            SerialPort.Open();
            SerialPort.DataReceived += ProcessInput;
            IsInitialized = true;
        }

        /// <summary>
        /// Indicate whether a result was ready before timeout.
        /// </summary>
        /// <returns></returns>
        private bool WaitForResultToBeReady()
        {
            return ResultReady.WaitOne(ReadTimeout);
        }

        public IDataFromDevice Execute(IDataToDevice command)
        {
            var timer = EasyStopwatch.StartMs();
            try
            {
                Init();

                var ncdFrame = new DataToDevice(command.Payload);
                SerialPort.Write(ncdFrame.BytesToTransmit());

                return WaitForResultToBeReady()
                    ? new DataFromDevice(Header, ByteCount, Payload.AsReadOnly(), Checksum)
                    : null;
            }
            finally
            {
                Console.WriteLine($"Execute took: {timer.Stop()} ms");
            }
        }

        public void Dispose()
        {
            SerialPort?.Dispose();
            ResultReady?.Dispose();
        }
    }

}
