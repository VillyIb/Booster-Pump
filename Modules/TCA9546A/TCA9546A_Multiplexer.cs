﻿using System;
using System.Collections.Generic;
using System.Linq;
using BoosterPumpLibrary.Settings;
using eu.iamia.BaseModule.Contract;
using eu.iamia.i2c.communication.contract;

// ReSharper disable UnusedMember.Global

namespace Modules.TCA9546A
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
    public class TCA9546A_Multiplexer 
    {
        private readonly IOutputModule ComModule;
        // See: https://media.ncd.io/sites/2/20170721134413/tca9546a.pdf

        public static byte DefaultAddressValue => 0x70;


        private readonly Register Setting0X00 = new(0x00, "Open channels", 1, Direction.Output);

        private IBitSetting ChannelSelection => Setting0X00.GetOrCreateSubRegister(4, 0, "Open Channels");

        protected  IEnumerable<IRegister> Registers => new List<IRegister> { Setting0X00 };

        public TCA9546A_Multiplexer(IOutputModule comModule)
        {
            ComModule = comModule;
            ComModule.SetupOnlyOnce(Registers, DefaultAddressValue);
        }

        /// <summary>
        /// Specify one or more channels {0...3} separated by , (comma).
        /// </summary>
        /// <param name="channels"></param>
        public void SelectOpenChannels(params MultiplexerChannels[] channels)
        {
            var aggregateBitPattern = channels.Aggregate<MultiplexerChannels, byte>(0, (current, channel) => (byte)(current | (byte)channel));
            ChannelSelection.Value = aggregateBitPattern;
            ComModule.Send();
        }
    }
}