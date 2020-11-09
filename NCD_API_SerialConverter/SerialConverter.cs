namespace NCD_API_SerialConverter
{
    using NCD_API_SerialConverter.Contracts;

    public class SerialConverter
    {
        private readonly INCD_API_SerialPort serialPort;

        public SerialConverter(INCD_API_SerialPort serialPort)
        {
            this.serialPort = serialPort;
        }

        public void Execute(IX ncdApiCommand)
        {
            var data = ncdApiCommand.ApiEncodedData();
            serialPort.Write(data);
        }
    }
}

