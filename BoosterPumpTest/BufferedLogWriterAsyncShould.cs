using BoosterPumpLibrary.Logger;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BoosterPumpTest
{
    public class BufferedLogWriterAsyncShould
    {
        BufferedLogWriterAsync Sut;
        IOutputFileHandler fakeFileHandler;
        CancellationTokenSource TokenSource;
        ControllerTest ControllerTest;

        public BufferedLogWriterAsyncShould()
        {
            fakeFileHandler = Substitute.For<IOutputFileHandler>();
            Sut = new BufferedLogWriterAsync(fakeFileHandler);

            TokenSource = new CancellationTokenSource();
            ControllerTest = new ControllerTest();
        }

        private async Task RequestCancellationAfterDelay()
        {
            await Task.Delay((int)(3 * 60 * 1000));
            TokenSource.Cancel();
        }

        [Fact]
        public async Task ExececuteShouldReturn15RowsIn30Seconds()
        {
            var stopTask = RequestCancellationAfterDelay();

            // Arrange
            var controllerTask = ControllerTest.ExecuteAsync(TokenSource.Token, Sut);

            // Act
            var writerTask = Sut.ExecuteAsync(TokenSource.Token);

            await Task.WhenAll(stopTask, controllerTask, writerTask);

            // Assert
            await fakeFileHandler.Received(15).WriteLine(Arg.Any<DateTime>(), Arg.Any<string>());

        }

        [Fact(Timeout =200000)]
        public async Task AssertExececuteShouldReturn1RowsIn30Seconds()
        {
            var probe1 = BufferedLogWriterAsync.RoundToMinute(DateTime.UtcNow.AddSeconds(15));
            var probe2 = probe1.AddMinutes(1);
            var probe3 = probe2.AddMinutes(1);

            var stopTask = RequestCancellationAfterDelay();

            // Arrange
            var controllerTask = ControllerTest.ExecuteAsync(TokenSource.Token, Sut);

            // Act
            var writerTask = Sut.AggregateExecuteAsync(TokenSource.Token);

            await Task.WhenAll(stopTask, controllerTask, writerTask);

            // Assert
            await fakeFileHandler.Received().WriteLine(Arg.Any<DateTime>(), Arg.Any<string>());
            await fakeFileHandler.Received(1).WriteLine(Arg.Is<DateTime>(probe1), Arg.Any<string>());
            await fakeFileHandler.Received(1).WriteLine(Arg.Is<DateTime>(probe2), Arg.Any<string>());
            await fakeFileHandler.Received(1).WriteLine(Arg.Is<DateTime>(probe3), Arg.Any<string>());

        }

    }
}

