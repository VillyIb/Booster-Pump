using System.Threading;
using System.Threading.Tasks;
using BoosterPumpLibrary.Logger;

namespace BoosterPumpApplication
{
    public interface IController
    {
        Task ExecuteAsync(CancellationToken cancellationToken, IBufferedLogWriter logger);
    }
}