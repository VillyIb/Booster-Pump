using BoosterPumpLibrary.Commands;
using NCD_API_SerialConverter.Contracts;
using SerialConverter.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace SerialConverter
{
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

            var expected = new byte[] { 0xAA, 0x08, 0xBE, 0x89, 0x01, 0x02, 0x03, 0xC, 0x0E, 0x0F, 0x28 };

            serialPort.Write(expected);
        }

        // 0xAA, 0x08, 0xBE, 0x89, 0x01, 0x02, 0x03, 0xC, 0x0E, 0x0F, 0x28 
    }
}

    