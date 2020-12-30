using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        readonly AutoResetEvent ResultReady = new AutoResetEvent(false);

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
                                ResultReady.Set();
                                Console.WriteLine("-- Finished reading");
                                break;
                            }
                        case NcdState.Overflow:
                            break;
                        default:
                            break;
                    }
                }
            };
        }

        private DataFromDevice DataFromDevice { get; set; }

        private void ClearInputAndQueue()
        {
            Console.WriteLine($"\nState: {State:G}");
            if (NcdState.Overflow != State && NcdState.Undefined != State)
            {
                // reading in progress
                //Console.WriteLine("waiting for reading to finish.");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var result = ResultReady.WaitOne(1000) ? "Finished reading" : "Timeout";
                stopwatch.Stop();
                Console.WriteLine($"Reading intput '{result}' duration: {stopwatch.ElapsedTicks} ticks");
            }

            State = NcdState.Undefined;
            DataFromDevice = null;
            //Queue.Clear();
        }

        public void Write(IEnumerable<byte> byteSequence)
        {
            ClearInputAndQueue();
            var stopwatch = new Stopwatch();
            var output = byteSequence.ToArray();
            stopwatch.Start();
            SerialPortSelected.Write(output, 0, output.Length);
            var received2 = ResultReady.WaitOne(1000) ? $"Finished reading in {stopwatch.ElapsedTicks} ticks" : "timeout";
            stopwatch.Stop();
            Console.WriteLine($"{received2}: waited: {stopwatch.ElapsedMilliseconds} ms");
            //Thread.Sleep(50);
        }


        private NcdState State;

        private bool WaitForResponseToBeReady()
        {
            var responseReady = ResultReady.WaitOne(5000);
            return responseReady;
        }

        public DataFromDevice Read()
        {
            do
            {
                if (Queue.TryDequeue(out var current))
                {
                    return current;
                }
            } while (WaitForResponseToBeReady());

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
