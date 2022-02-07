using Microsoft.Extensions.DependencyInjection;

namespace eu.iamia.SerialPortSetting.Contract
{
    public interface IController
    {
        void Init(IServiceScope scope);

        void Execute(IBufferedLogWriter logger);

        IMeasurementSettings MeasurementSettings { get; }
    }
}