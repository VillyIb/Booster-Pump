using Microsoft.Extensions.DependencyInjection;
// ReSharper disable UnusedMemberInSuper.Global

namespace eu.iamia.SerialPortSetting.Contract
{
    public interface IController
    {
        void Init(IServiceScope scope);

        void Execute(IBufferedLogWriter logger);

        IMeasurementSettings MeasurementSettings { get; }
    }
}