using BoosterPumpLibrary.Logger;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BoosterPumpApplicationAsync
{
    public class ControlAsync
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
