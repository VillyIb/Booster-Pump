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

        public IDataFromDevice Execute(NCD_API_Converter_Hard_Reboot_Command command)
        {
            return Execute(command.ApiEncodedData());
        }

        public IDataFromDevice Execute(NCD_API_Converter_Soft_Reboot_Command command)
        {
            return Execute(command.ApiEncodedData());
        }

        public IDataFromDevice Execute(NCD_API_Converter_Stop_Command command)
        {
            return Execute(command.ApiEncodedData());
        }

        public IDataFromDevice Execute(NCD_API_Converter_Test2Way_Command command)
        {
            return Execute(command.ApiEncodedData());
        }

        public IDataFromDevice Execute(NCD_API_Converter_Scan_Command command)
        {
            return Execute(command.ApiEncodedData());
        }
    }
}

