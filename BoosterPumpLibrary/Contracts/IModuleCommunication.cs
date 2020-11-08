using BoosterPumpLibrary.Commands;

namespace BoosterPumpLibrary.Contracts
{
    public interface IModuleCommunication
    {
        IDataFromDevice Execute(HardRebootCommand command);

        IDataFromDevice Execute(ReadCommand command);

        IDataFromDevice Execute(ScanCommand command);

        IDataFromDevice Execute(SoftRebootCommand command);

        IDataFromDevice Execute(StopCommand command);

        IDataFromDevice Execute(Test2WayCommand command);

        IDataFromDevice Execute(WriteCommand command);

        IDataFromDevice Execute(WriteReadCommand command);
    }
}
