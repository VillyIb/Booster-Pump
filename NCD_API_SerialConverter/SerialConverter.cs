namespace NCD_API_SerialConverter
{
    using BoosterPumpLibrary.Commands;
    using BoosterPumpLibrary.Contracts;
    using NCD_API_SerialConverter.Contracts;
    using NCD_API_SerialConverter.NcdApiProtocol.DeviceCommands;
    using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;
    using System.Collections.Generic;

    public class SerialConverter : ISerialConverter // ,  IModuleCommunication
    {
        private readonly INCD_API_SerialPort serialPort;

        public SerialConverter(INCD_API_SerialPort serialPort)
        {
            this.serialPort = serialPort;
        }

        protected IDataFromDevice Execute(IEnumerable<byte> data)
        {
            serialPort.Write(data);
            var returnValue = serialPort.Read();
            return returnValue;
        }

        public IDataFromDevice Execute(ReadCommand command)
        {
            IX ncdApiCommand = new DeviceRead(command);
            return Execute(ncdApiCommand.ApiEncodedData());
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

