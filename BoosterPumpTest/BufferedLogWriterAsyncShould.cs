using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;
using NSubstitute;
using BoosterPumpLibrary.Logger;

namespace BoosterPumpTest
{
    public class BufferedLogWriterAsyncShould
    {
        BufferedLogWriterAsync Sut;
        IOutputFileHandler fakeFileHandler;
        CancellationTokenSource TokenSource;
        ControllerTest ControllerTest;
        private static IConfiguration Configuration;

        public BufferedLogWriterAsyncShould()
        {
            // Arrange
            fakeFileHandler = Substitute.For<IOutputFileHandler>();
            Sut = new BufferedLogWriterAsync(fakeFileHandler);
            TokenSource = new CancellationTokenSource();
            ControllerTest = new ControllerTest();
        }

        private async Task RequestCancellationAfterDelay()
        {
            await Task.Delay((int)(2 * 60 * 1000));
            Console.WriteLine($"\r\nDelay Ended {DateTime.Now}");
            TokenSource.Cancel();
        }

        [Fact(Timeout = 250000, Skip = "Too time consuming")]
        //[Fact(Timeout = 250000)]
        public async Task AssertExecuteShouldReturnXRowsInMSeconds()
        {

            var now = DateTime.UtcNow;
            var delay = BufferedLogWriterAsync.RoundToMinute(now).AddSeconds(75).Subtract(now);
            await Task.Delay(delay);
            Console.WriteLine($"\r\n\n---------------------------------------------------------------------------------------\n\r{DateTime.Now}");

            var probe1 = BufferedLogWriterAsync.RoundToMinute(DateTime.UtcNow.AddSeconds(15));
            var probe2 = probe1.AddMinutes(1);
            var probe3 = probe2.AddMinutes(1);
            var probe4 = probe3.AddMinutes(1);

            var stopTask = RequestCancellationAfterDelay();

            // Act
            var writerTask = Sut.AggregateExecuteAsync(TokenSource.Token);
            var controllerTask = ControllerTest.ExecuteAsync(TokenSource.Token, Sut);

            Console.WriteLine("Waiting for all to finish");
            await Task.WhenAll(stopTask, controllerTask, writerTask);

            // Assert
            await fakeFileHandler.Received().WriteLineAsync(Arg.Any<DateTime>(), Arg.Any<string>(), Arg.Any<string>());
            await fakeFileHandler.Received(1).WriteLineAsync(Arg.Is<DateTime>(probe2), Arg.Any<string>(), Arg.Any<string>());
            await fakeFileHandler.Received(1).WriteLineAsync(Arg.Is<DateTime>(probe3), Arg.Any<string>(), Arg.Any<string>());
            await fakeFileHandler.Received(1).WriteLineAsync(Arg.Is<DateTime>(probe4), Arg.Any<string>(), Arg.Any<string>());
        }
    }
}

