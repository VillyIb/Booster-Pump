using System;
using System.Collections.Generic;
using System.Linq;
using BoosterPumpLibrary.Commands;
using BoosterPumpLibrary.Contracts;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Settings;


namespace Modules
{
    public class TCA9546MultiplexerModule : BaseModuleV2
    {
        // See: https://media.ncd.io/sites/2/20170721134413/tca9546a.pdf

        public override byte DefaultAddress => 0x70;

        // TODO make strongly typed. using enum with flags.
        public const int Channel0 = BitPattern.D0;
        public const int Channel1 = BitPattern.D1;
        public const int Channel2 = BitPattern.D2;
        public const int Channel3 = BitPattern.D3;

        private readonly Register Setting0X00 = new BoosterPumpLibrary.Settings.Register(0x00, "Open channels", 1);

        private BitSetting ChannelSelection => Setting0X00.GetOrCreateSubRegister(4, 0, "Open Channels");

        protected override IEnumerable<RegisterBase> Registers => new List<RegisterBase> { Setting0X00 };

        public TCA9546MultiplexerModule(ISerialConverter serialPort) : base(serialPort)
        { }

        /// <summary>
        /// Specify one or more channels {0...3} separated by , (comma).
        /// </summary>
        /// <param name="bitPattern"></param>
        public void SelectOpenChannels(params byte[] bitPattern) // TODO make strongly typed 
        {
            byte aggregateBitPattern = bitPattern.Aggregate<byte, byte>(0x00, (current1, current) => (byte)(current1 | current));

            ChannelSelection.Value = aggregateBitPattern;
            Send();
        }

        public override void Init()
        { }

    }
}
