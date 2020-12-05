using System;
using System.Threading;
using NCD_API_SerialConverter.NcdApiProtocol;

namespace NCD_API_SerialConverter
{
    using BoosterPumpLibrary.Commands;
    using BoosterPumpLibrary.Contracts;
    using Contracts;
    using NcdApiProtocol.DeviceCommands;
    using NcdApiProtocol.SerialConverterCommands;
    using System.Collections.Generic;

    public class SerialConverter : ISerialConverter // ,  IModuleCommunication
    {
        private readonly INcdApiSerialPort SerialPort;

        public SerialConverter(INcdApiSerialPort serialPort)
        {
            this.SerialPort = serialPort;
        }

        protected IDataFromDevice Execute(IEnumerable<byte> data)
        {
            SerialPort.Write(data);
            Thread.Sleep(5);
            var returnValue = SerialPort.Read();
            return returnValue;
        }

        public IDataFromDevice Execute(ReadCommand command)
        {
            try
            {
                IX ncdApiCommand = new DeviceRead(command);
                return Execute(ncdApiCommand.ApiEncodedData());
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine($"Command: {command}, Error: {ex.Message}");
                throw;
            }
        }

        public IDataFromDevice Execute(WriteCommand command)
        {
            IX ncdApiCommand = new DeviceWrite(command);
            return Execute(ncdApiCommand.ApiEncodedData());
        }

        public IDataFromDevice Execute(WriteReadCommand command)
        {
            IX ncdApiCommand = new DeviceWriteRead(command);
            return Execute(ncdApiCommand.ApiEncodedData());
        }

        public IDataFromDevice Execute(ConverterBase command)
        {
            return Execute(command.ApiEncodedData());
        }
    }
}

