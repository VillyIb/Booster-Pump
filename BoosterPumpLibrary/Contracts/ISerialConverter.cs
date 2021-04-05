// ReSharper disable UnusedMemberInSuper.Global

using System;

namespace BoosterPumpLibrary.Contracts
{
    using Commands;

    [Obsolete]
    public interface ISerialConverter
    {
        IDataFromDevice Execute(ReadCommand command);

        IDataFromDevice Execute(WriteCommand command);

        IDataFromDevice Execute(WriteReadCommand command);
    }
}

