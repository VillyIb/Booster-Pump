using System;
using System.Collections.Generic;
using System.Linq;
using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.ModuleBase;

namespace BoosterPumpLibrary.Modules
{
    public class TCA9546MultiplexerModule : BaseModule
    {
        public override byte Address => 0x70;

        protected override IEnumerable<Register> Registers => new List<Register>(0);

        private readonly Register OpenChannels = new Register(0x00, "Open channels", "X");

        public TCA9546MultiplexerModule(ISerialConverter serialPort) : base(serialPort)
        { }

        public void SelectOpenChannels(byte bitPattern)
        {
            OpenChannels.SetDataRegister(bitPattern);
            var writeCommand = new WriteCommand { Address = Address, Payload = new byte[] { OpenChannels.Value } };
            var status = SerialPort.Execute(writeCommand);
            if (status.Payload.First() != 0x55)
            {
                throw new ApplicationException("Unable to select open ports.");
            }
        }

        public override void Init()
        {
            throw new NotImplementedException();
        }
    }
}
