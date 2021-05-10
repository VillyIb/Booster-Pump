using System;
using System.Collections.Generic;
using System.Linq;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Settings;
using eu.iamia.NCD.API.Contract;

namespace Modules
{
    /// <summary>
    /// Select one or more concatenating with '|' [None|Channel0|Channel1|Channel2|Channel3]
    /// </summary>
    [Flags]
    public enum MultiplexerChannels
    {
        None = 0,
        Channel0 = 1,
        Channel1 = 2,
        Channel2 = 4,
        Channel3 = 8
    }

    // ReSharper disable once InconsistentNaming
    public class TCA9546MultiplexerModule : BaseModuleV2
    {
        // See: https://media.ncd.io/sites/2/20170721134413/tca9546a.pdf

        public static byte DefaultAddressValue => 0x70;

        public override byte DefaultAddress => DefaultAddressValue;

        private readonly Register Setting0X00 = new Register(0x00, "Open channels", 1);

        private BitSetting ChannelSelection => Setting0X00.GetOrCreateSubRegister(4, 0, "Open Channels");

        protected override IEnumerable<RegisterBase> Registers => new List<RegisterBase> { Setting0X00 };

        public TCA9546MultiplexerModule(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        { }

        /// <summary>
        /// Specify one or more channels {0...3} separated by , (comma).
        /// </summary>
        /// <param name="channels"></param>
        public void SelectOpenChannels(params MultiplexerChannels[] channels)
        {
            var aggregateBitPattern = channels.Aggregate<MultiplexerChannels, byte>(0, (current, channel) => (byte)(current | (byte)channel));
            ChannelSelection.Value = aggregateBitPattern;
            Send();
        }
    }
}
