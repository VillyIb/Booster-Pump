using System;
using System.Collections.Generic;
using System.Linq;
using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.ModuleBase;

namespace Modules
{
    public class TCA9546MultiplexerModule : BaseModule
    {
        public override byte DefaultAddress => 0x70;

        // TODO make strongly typed. using enum with flags.
        public const int Channel0 = BitPattern.D0;
        public const int Channel1 = BitPattern.D1;
        public const int Channel2 = BitPattern.D2;
        public const int Channel3 = BitPattern.D3;

        protected override IEnumerable<RegisteBase> Registers => new List<RegisteBase>(0);

        private readonly RegisteBase OpenChannels = new RegisteBase(0x00, "Open channels", "X");

        public TCA9546MultiplexerModule(ISerialConverter serialPort) : base(serialPort)
        { }

        /// <summary>
        /// Specify one or more channels {0...3} separated by , (comma).
        /// </summary>
        /// <param name="bitPattern"></param>
        public void SelectOpenChannels(params byte[] bitPattern) // TODO make strongly typed 
        {
            byte aggregateBitPattern = bitPattern.Aggregate<byte, byte>(0x00, (current1, current) => (byte) (current1 | current));
            OpenChannels.SetDataRegister(aggregateBitPattern);
            var writeCommand = new WriteCommand { Address = Address, Payload = new[] { OpenChannels.Value } };
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
