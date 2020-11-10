namespace BoosterPumpLibrary.Contracts
{
    using BoosterPumpLibrary.Commands;

    public interface ISerialConverter
    {
        IDataFromDevice Execute(ReadCommand command);

        IDataFromDevice Execute(WriteCommand command);

        IDataFromDevice Execute(WriteReadCommand command);
    }
}

