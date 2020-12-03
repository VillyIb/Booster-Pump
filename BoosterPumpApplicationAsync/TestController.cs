using BoosterPumpApplication;
using BoosterPumpLibrary.Logger;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BoosterPumpApplicationAsync
{
    public class TestController : IController
    {
        public async Task ExecuteAsync(CancellationToken cancellationToken, IBufferedLogWriter logger)
        {
            try
            {
                do
                {
                    logger.Add("xxx", DateTime.UtcNow);
                    await Task.Delay(2000, cancellationToken);
                } while (true);
            }
            catch (OperationCanceledException)
            { }
        }
    }
}
