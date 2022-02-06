﻿using System;
using System.Collections.Generic;
using System.Threading;
using eu.iamia.NCD.Shared;
using eu.iamia.ReliableSerialPort;
using eu.iamia.NCD.API.Contract;

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

        public void Dispose()
        {
            SerialPort?.Dispose();
            ResultReady?.Dispose();
        }

        public INcdApiProtocol Execute(INcdApiProtocol i2CCommand)
        {
            Init();

            SerialPort.Write(i2CCommand.GetApiEncodedData());

            //Thread.Sleep(100);

            if (WaitForResultToBeReady()) { eu.iamia.NCD.API.Contract.INcdApiProtocol result = new NcdApiProtocol(Header, ByteCount, Payload, Checksum); }

            return null;
        }
    }

}
