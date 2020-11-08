using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SerialConverter.Test
{
    using NSubstitute;
    using Commands;
    using NCD_API_SerialConverter.Contracts;
    using BoosterPumpLibrary.Commands;
    using System.Net.Http.Headers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class NCD_API_SerialConverterShould
    {
        INCD_API_SerialPort fakeSerialPort;
        SerialConverter sud;

        private byte Address { get; set; }

        private byte[] Payload { get; set; }

        private byte Lenght { get; set; }

        private byte Command { get; set; }

        private byte Checksum { get; set; }
        public NCD_API_SerialConverterShould()
        {
            fakeSerialPort = Substitute.For<INCD_API_SerialPort>();
            sud = new SerialConverter(fakeSerialPort);

            Address = 0x41;
            Payload = new byte[] { 0x01, 0x02, 0x03, 0xC, 0x0E, 0x0F };
        }



        private IEnumerable<byte> Expected()
        {
            yield return 0xAA;
            yield return Lenght;
            yield return Command;
            yield return Address;
            foreach(var current in Payload)
            {
                yield return current;
            }
            yield return Checksum;           
        }



        [Fact]
        public void WriteByteSequenceForEcecuteWriteCommand()
        {
           
            Command = 0xBE;
            Lenght = (byte)(Payload.Length + 2);
            Checksum = 0xE0;

            var writeCommand = new WriteCommand { Address = Address, Payload = Payload };
            var ncdCommand = new NCD_API_Packet_Write_Command(writeCommand);
            sud.Execute(ncdCommand);

            CollectionAssert.AreEqual(Expected().ToList(), ncdCommand.ApiEncodedData().ToList());


            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => Expected().ToArray().SequenceEqual(seq.ToArray())));
        }


        [Fact]
        public void WriteByteSequenceForEcecuteReadCommand()
        {

            Command = 0xBF;
            Payload = new byte[] { 0x05 };
            Lenght = (byte)(Payload.Length + 2);
            Checksum = 0xFA;

            var command = new ReadCommand { Address = Address, LengthRequested = 5 };
            var ncdCommand = new NCD_API_Packet_Read_Command(command);
            sud.Execute(ncdCommand);

            var exp = Expected().ToArray();

            fakeSerialPort.Received().Write(Arg.Any<IEnumerable<byte>>());
            fakeSerialPort.Received().Write(Arg.Is<IEnumerable<byte>>(seq => Expected().ToArray().SequenceEqual(seq)));
        }

    }
}
