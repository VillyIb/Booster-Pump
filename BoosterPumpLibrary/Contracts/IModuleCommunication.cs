using BoosterPumpLibrary.Commands;

namespace BoosterPumpLibrary.Contracts
{
    public interface IModuleCommunication
    {
        IDataFromDevice Execute(WriteCommand command);
    }
}
