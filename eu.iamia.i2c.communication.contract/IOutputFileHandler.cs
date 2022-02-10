using System;

namespace eu.iamia.i2c.communication.contract
{

    public interface IOutputFileHandler
    {
        /// <summary>
        /// Character og separate columns in file.
        /// </summary>
        char SeparatorCharacter { get; }

        void WriteLine(DateTime timestamp, string suffix, string line);

        void Close();
    }
}