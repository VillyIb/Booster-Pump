using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using eu.iamia.NCD.Serial.Contract;
using eu.iamia.NCD.Shared;
using eu.iamia.ReliableSerialPort;
using eu.iamia.Util;

namespace eu.iamia.NCD.Serial
{
    public class SerialGateway : IGateway
    {
        private static TimeSpan ReadTimeout => TimeSpan.FromSeconds(10); // TODO move to settings.

        private ISerialPortDecorator SerialPort { get; }

        public SerialGateway(ISerialPortDecorator serialPort)
        {
            SerialPort = serialPort;
        }

        private readonly AutoResetEvent ResultReady = new(false); 

        private NcdState State;

        private List<byte> Payload;

        private static byte Header => 0xAA;

        private byte ByteCount { get; set; }

        private byte Checksum { get; set; }

        private void ProcessInput(object sender, DataReceivedArgs args)
        {
            foreach (var current in args.Data)
            {
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
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
                            Payload = new(ByteCount);
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
                }
            }
        }

        private bool IsInitialized;

        public List<byte> xReceived = new List<byte>();

        private void Init()
        {
            State = NcdState.ExpectHeader;
            ByteCount = byte.MinValue;
            Payload = null;
            Checksum = byte.MinValue;

            if (IsInitialized) return;

            SerialPort.Open();
            SerialPort.DataReceived += ProcessInput;
            SerialPort.DataReceived += (_, args) => { xReceived.AddRange(args.Data); };
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

        public void Dispose()
        {
            SerialPort?.Dispose();
            ResultReady?.Dispose();
        }

        public INcdApiProtocol Execute(INcdApiProtocol i2CCommand)
        {
            var timer = EasyStopwatch.StartMs();
            try
            {
                Init();

                SerialPort.Write(i2CCommand.GetApiEncodedData());

                //Thread.Sleep(100);
                
                if (WaitForResultToBeReady())
                    return new NcdApiProtocol(Header, ByteCount, Payload, Checksum);
                else
                    return (NcdApiProtocol)null;
            }
            finally
            {
                //Console.WriteLine($"Execute took: {timer.Stop()} ms");
            }
        }
    }

}
