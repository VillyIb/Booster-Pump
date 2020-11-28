using BoosterPumpLibrary.Logger;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BoosterPumpTest
{
    public class ControllerTest
    {
        public async Task ExecuteAsync(CancellationToken cancellationToken, IBufferedLogWriter logger)
        {
            try
            {
                do
                {
                    var now = DateTime.UtcNow;
                    logger.Add($"{now}", now);
                    await Task.Delay(2000);
                } while (!cancellationToken.IsCancellationRequested);
            }
            catch (OperationCanceledException)
            { 
            
            }
        }
    }
}
