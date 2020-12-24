using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using BoosterPumpLibrary.Logger;
using Xunit.Abstractions;

namespace BoosterPumpTest
{
    public class BufferedLogWriterV2Should
    {
        private readonly ITestOutputHelper TestOutputHelper;
        readonly BufferedLogWriterV2 Sut;
        readonly IOutputFileHandler FakeFileHandler;
        readonly CancellationTokenSource TokenSource;
        readonly ControllerTest ControllerTest;

        public BufferedLogWriterV2Should(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;
            // Arrange
            FakeFileHandler = Substitute.For<IOutputFileHandler>();
            Sut = new BufferedLogWriterV2(FakeFileHandler);
            TokenSource = new CancellationTokenSource();
            ControllerTest = new ControllerTest();
        }

        private async Task RequestCancellationAfterDelay()
        {
            await Task.Delay(2 * 60 * 1000);
            Console.WriteLine($"\r\nDelay Ended {DateTime.Now}");
            TokenSource.Cancel();
        }

        [Fact(Timeout = 250000, Skip = "Too time consuming")]
        //[Fact(Timeout = 250000)]
        public async Task AggregateExecuteAsync_WritesLineToFile()
        {
            var now = DateTime.UtcNow;
            var delay = BufferedLogWriterV2.RoundToMinute(now).AddSeconds(75).Subtract(now);
            await Task.Delay(delay);
            TestOutputHelper.WriteLine($"\r\n\n---------------------------------------------------------------------------------------\n\r{DateTime.Now}");

            var probe1 = BufferedLogWriterV2.RoundToMinute(DateTime.UtcNow.AddSeconds(15));
            var probe2 = probe1.AddMinutes(1);
            var probe3 = probe2.AddMinutes(1);
            var probe4 = probe3.AddMinutes(1);

            var stopTask = RequestCancellationAfterDelay();

            // Act
            var writerTask = Sut.AggregateExecuteAsync(TokenSource.Token);
            var controllerTask = ControllerTest.ExecuteAsync(TokenSource.Token, Sut);

            TestOutputHelper.WriteLine("Waiting for all to finish");
            await Task.WhenAll(stopTask, controllerTask, writerTask);

            // Assert
            await FakeFileHandler.Received().WriteLineAsync(Arg.Any<DateTime>(), Arg.Any<string>(), Arg.Any<string>());
            await FakeFileHandler.Received(1).WriteLineAsync(Arg.Is(probe2), Arg.Any<string>(), Arg.Any<string>());
            await FakeFileHandler.Received(1).WriteLineAsync(Arg.Is(probe3), Arg.Any<string>(), Arg.Any<string>());
            await FakeFileHandler.Received(1).WriteLineAsync(Arg.Is(probe4), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void IsNextMinute_ReturnsTrueThenFalse()
        {
            Assert.True(Sut.IsNextMinute());
            Assert.False(Sut.IsNextMinute());
        }

        [Fact]
        public void RoundToMinute_RemovesSecondsAndFractions()
        {
            var timestamp = DateTime.Today.AddHours(12).AddMinutes(12);
            Assert.Equal(timestamp, BufferedLogWriterV2.RoundToMinute(timestamp.AddSeconds(12).AddMilliseconds(12)));
        }

        [Fact]
        public void AggregateExecute_()
        {
            Sut.AggregateExecute();
            Assert.True(false);
        }

        [Fact]
        public void AggregateFlush_()
        {
            Sut.AggregateFlush(DateTime.UtcNow);
            Assert.True(false);
        }

        [Fact]
        public void AggregateFlushUnconditionally_()
        {
            Sut.AggregateFlushUnconditionally();
            Assert.True(false);
        }
    }
}

