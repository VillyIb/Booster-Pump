// ReSharper disable UnusedMemberInSuper.Global
namespace BoosterPumpLibrary.Contracts
{
    using Commands;

    public interface ISerialConverter
    {
        IDataFromDevice Execute(ReadCommand command);

        IDataFromDevice Execute(WriteCommand command);

        IDataFromDevice Execute(WriteReadCommand command);
    }
}

