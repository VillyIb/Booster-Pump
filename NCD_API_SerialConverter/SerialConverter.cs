namespace NCD_API_SerialConverter
{
    using BoosterPumpLibrary.Commands;
    using BoosterPumpLibrary.Contracts;
    using NCD_API_SerialConverter.Commands;
    using NCD_API_SerialConverter.Contracts;
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
            IX ncdApiCommand = new NCD_API_Packet_Read_Command(command);
            return Execute(ncdApiCommand.ApiEncodedData());
        }

        public IDataFromDevice Execute(WriteCommand command)
        {
            IX ncdApiCommand = new NCD_API_Packet_Write_Command(command);
            return Execute(ncdApiCommand.ApiEncodedData());
        }

        public IDataFromDevice Execute(WriteReadCommand command)
        {
            IX ncdApiCommand = new NCD_API_Packet_Write_Read_Command(command);
            return Execute(ncdApiCommand.ApiEncodedData());
        }

        public IDataFromDevice Execute(ConverterHardReboot command)
        {
            return Execute(command.ApiEncodedData());
        }

        public IDataFromDevice Execute(ConverterSoftReboot command)
        {
            return Execute(command.ApiEncodedData());
        }

        public IDataFromDevice Execute(ConverterStop command)
        {
            return Execute(command.ApiEncodedData());
        }

        public IDataFromDevice Execute(ConverterTest2Way command)
        {
            return Execute(command.ApiEncodedData());
        }

        public IDataFromDevice Execute(ConverterScan command)
        {
            return Execute(command.ApiEncodedData());
        }
    }
}

