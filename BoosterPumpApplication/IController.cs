using System.Threading;
using System.Threading.Tasks;
using BoosterPumpLibrary.Logger;

namespace BoosterPumpApplication
{
    public interface IController
    {
        void Execute(IBufferedLogWriter logger);

        Task ExecuteAsync(CancellationToken cancellationToken, IBufferedLogWriter logger);
    }
}