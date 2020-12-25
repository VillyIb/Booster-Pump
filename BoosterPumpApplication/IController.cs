using BoosterPumpConfiguration;
using BoosterPumpLibrary.Logger;
using Microsoft.Extensions.DependencyInjection;

namespace BoosterPumpApplication
{
    public interface IController
    {
        void Init(IServiceScope scope);

        void Execute(IBufferedLogWriter logger);

        MeasurementSettings MeasurementSettings { get; }
    }
}