using System;

// ReSharper disable UnusedMemberInSuper.Global

namespace eu.iamia.SerialPortSetting.Contract
{
    public interface IBufferedLogWriter
    {
        event EventHandler Disposed;

        void Add(IBufferLine payload);


        void AggregateFlush(DateTime window);

        void AggregateFlushUnconditionally();

        //Task AggregateFlushAsync(DateTime window);

        //Task AggregateFlushUnconditionalAsync();

        //Task WaitUntilSecond02InNextMinuteAsync();

        bool IsNextMinute();
    }
}
