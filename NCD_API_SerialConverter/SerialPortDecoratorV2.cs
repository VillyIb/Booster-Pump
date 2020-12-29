using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using BoosterPumpConfiguration;
using Microsoft.Extensions.Options;
using NCD_API_SerialConverter.Contracts;
using NCD_API_SerialConverter.NcdApiProtocol;

// see:https://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport
// see:https://eliot-jones.com/2016/11/csharp-serial-port

namespace NCD_API_SerialConverter
{
    public class SerialPortDecoratorV2 : INcdApiSerialPort, IDisposable
    {
        protected ReliableSerialPortV2 SerialPortSelected { get; private set; }

        protected IReadNcdApiFormat ReadUtil { get; }

        private SerialPortSettings SerialPortSettings { get; }

        private readonly ConcurrentQueue<DataFromDevice> Queue;

        protected int ReadByte()
        {
            throw new NotFiniteNumberException();
        }

        protected IEnumerable<byte> ReadBlock()
        {
            throw new NotFiniteNumberException();
        }


        public SerialPortDecoratorV2(IOptions<SerialPortSettings> settings)
        {
            SerialPortSettings = settings.Value;
            ReadUtil = new ReadNcdApiFormatV2(ReadByte, ReadBlock);
        }

        public SerialPortDecoratorV2(SerialPortSettings settings)
        {
            SerialPortSettings = settings;
            ReadUtil = new ReadNcdApiFormatV2(ReadByte, ReadBlock);
            Queue = new ConcurrentQueue<DataFromDevice>();
        }

        private List<byte> Payload;

        public void Open()
        {
            var ports = SerialPort.GetPortNames().ToList();
            var port = ports.Last();
            Console.WriteLine($"Serial port: {port}, Speed: {SerialPortSettings.BaudRate} bps, Timeout: {SerialPortSettings.Timeout} ms");
            if (null != SerialPortSelected && SerialPortSelected.IsOpen) { SerialPortSelected.Close(); }

            SerialPortSelected = new ReliableSerialPortV2(port, SerialPortSettings.BaudRate)
            {
                ReadTimeout = SerialPortSettings.Timeout
            };
            SerialPortSelected.Open();

            SerialPortSelected.DataReceived += (sender, args) =>
            {

                foreach (var current in args.Data)
                {
                    if (DataFromDevice is null)
                    {
                        DataFromDevice = new DataFromDevice();
                        State = NcdState.ExpectHeader;
                    }

                    switch (State)
                    {
                        case NcdState.Undefined:
                            break;
                        case NcdState.ExpectHeader:
                            {
                                if (0xAA == current)
                                {
                                    DataFromDevice.Header = current;
                                    State = NcdState.ExpectLength;
                                }
                                break;
                            }
                        case NcdState.ExpectLength:
                            {
                                DataFromDevice.ByteCount = current;
                                Payload = new List<byte>(DataFromDevice.ByteCount);
                                State = NcdState.ExpectPayload;
                                break;
                            }
                        case NcdState.ExpectPayload:
                            {
                                Payload.Add(current);
                                if (DataFromDevice.ByteCount <= Payload.Count)
                                {
                                    DataFromDevice.Payload = Payload.ToArray();
                                    State = NcdState.ExpectChecksum;
                                }
                                break;
                            }
                        case NcdState.ExpectChecksum:
                            {
                                DataFromDevice.Checksum = current;
                                State = NcdState.Overflow;
                                Queue.Enqueue(DataFromDevice);
                                DataFromDevice = null;
                                autoEvent.Set();
                                break;
                            }
                        case NcdState.Overflow:
                            break;
                        default:
                            break;
                    }

                    //Queue.Enqueue(current);
                }


            };
        }

        private DataFromDevice DataFromDevice { get; set; }




        public void Write(IEnumerable<byte> byteSequence)
        {
            DataFromDevice = null;

            var output = byteSequence.ToArray();

            SerialPortSelected.Write(output, 0, output.Length);
        }

        AutoResetEvent autoEvent = new AutoResetEvent(false);

        private NcdState State;

        private bool WaitForResponseToBeReady()
        {
           var responseReady =  autoEvent.WaitOne(5000);
           return responseReady;
        }

        public DataFromDevice Read()
        {
            if (WaitForResponseToBeReady())
            {
                DataFromDevice current;
                if (Queue.TryDequeue(out current))
                {
                    return current;
                }
            }

            return null;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public enum NcdState
    {
        Undefined = 0,
        ExpectHeader = 1,
        ExpectLength = 2,
        ExpectPayload = 3,
        ExpectChecksum = 4,
        Overflow = 5
    }
}
