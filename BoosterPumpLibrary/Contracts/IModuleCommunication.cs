using BoosterPumpLibrary.Commands;

namespace BoosterPumpLibrary.Contracts
{
    public interface IModuleCommunication
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        IDataFromDevice Execute(ReadCommand command); // TODO what about other commands? ! Used by UnitTests.

        IDataFromDevice Execute(WriteCommand command); // TODO what about other commands? ! Used by UnitTests.

        IDataFromDevice Execute(WriteReadCommand command); // TODO what about other commands? ! Used by UnitTests.


    }
}
