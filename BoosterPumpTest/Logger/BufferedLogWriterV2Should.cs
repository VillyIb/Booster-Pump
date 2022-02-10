using System;
using System.Threading.Tasks;
using BoosterPumpLibrary.Logger;
using eu.iamia.Util;
using eu.iamia.i2c.communication.contract;
using NSubstitute;
using Xunit;

namespace BoosterPumpLibrary.UnitTest.Logger
{
    public class BufferedLogWriterV2Should
    {
        private readonly BufferedLogWriterV2 Sut;
        private readonly IOutputFileHandler FakeFileHandler;

        public BufferedLogWriterV2Should()
        {
            // Arrange
            FakeFileHandler = Substitute.For<IOutputFileHandler>();
            Sut = new(FakeFileHandler);
        }

        [Fact]
        public async Task WaitUntilSecond02InNextMinuteAsync_WaitUntilNextMinute()
        {
            InitSystemDateTime(50, 55, 1D);
            await Sut.WaitUntilSecond02InNextMinuteAsync();
            var now = SystemDateTime.UtcNow;
            Assert.Equal(51, now.Minute);
            Assert.True(now.Second >= 2);
        }

        [Fact]
        public void IsNextMinute_ReturnTrueFirstTime()
        {
            Assert.True(Sut.IsNextMinute());
        }

        [Fact]
        public void IsNextMinute_ReturnFalseSecondTime()
        {
            InitSystemDateTime(55, 30, 1D);
            Sut.IsNextMinute();
            Assert.False(Sut.IsNextMinute());
        }

        [Fact]
        public void IsNextMinute_ReturnTrueAfterOneMinute()
        {
            InitSystemDateTime(55, 30, 1D);
            Sut.IsNextMinute();
            InitSystemDateTime(56, 30, 1D);
            Assert.True(Sut.IsNextMinute());
        }

        [Fact]
        public void RoundToMinute_RemovesSecondsAndFractions()
        {
            var timestamp = SystemDateTime.Today.AddHours(12).AddMinutes(12);
            Assert.Equal(timestamp, BufferedLogWriterV2.RoundToMinute(timestamp.AddSeconds(12).AddMilliseconds(12)));
        }

        private static void InitSystemDateTime(int minute, int second = 30, double amplification = 0D)
        {
            SystemDateTime.SetTime(new DateTime(2000, 01, 01, 12, minute, second, DateTimeKind.Utc), amplification);
        }

        private void SetupBufferLines()
        {
            InitSystemDateTime(10);
            Sut.Add(new BufferLine("A", SystemDateTime.UtcNow));
            InitSystemDateTime(11);
            Sut.Add(new BufferLine("B", SystemDateTime.UtcNow));
            Sut.Add(new BufferLine("C", SystemDateTime.UtcNow));
            InitSystemDateTime(12);
            Sut.Add(new BufferLine("D", SystemDateTime.UtcNow));
        }

        private void SetupBufferLineMeasurements()
        {
            InitSystemDateTime(10);
            Sut.Add(new BufferLineMeasurement(SystemDateTime.UtcNow, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f));
            InitSystemDateTime(11);
            Sut.Add(new BufferLineMeasurement(SystemDateTime.UtcNow, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f));
            Sut.Add(new BufferLineMeasurement(SystemDateTime.UtcNow, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f));
            InitSystemDateTime(12);
            Sut.Add(new BufferLineMeasurement(SystemDateTime.UtcNow, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f));
        }

        [Fact]
        public void AggregateFlushUnconditionally_WritesMessageWhenFailing()
        {
            InitSystemDateTime(10);
            Sut.Add(new BufferLineMeasurement(SystemDateTime.UtcNow, 1f, 2f));

            Sut.AggregateFlushUnconditionally();
            FakeFileHandler.Received(1).WriteLine(Arg.Any<DateTime>(), Arg.Is("T"), Arg.Any<string>());
            FakeFileHandler.Received(1).WriteLine(Arg.Any<DateTime>(), Arg.Is("M"), Arg.Any<string>());
        }

        [Fact]
        public void AggregateFlush_WriteOlderEntriesAggregated()
        {
            SetupBufferLines();
            Sut.AggregateFlush(SystemDateTime.UtcNow);
            FakeFileHandler.Received(1).WriteLine(Arg.Any<DateTime>(), Arg.Is("T"), Arg.Is("A, B, C"));
            FakeFileHandler.Received(1).WriteLine(Arg.Any<DateTime>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void AggregateFlushUnconditionally_WriteAllEntriesAggregated()
        {
            SetupBufferLines();
            Sut.AggregateFlushUnconditionally();
            FakeFileHandler.Received(1).WriteLine(Arg.Any<DateTime>(), Arg.Is("T"), Arg.Is("A, B, C, D"));
            FakeFileHandler.Received(1).WriteLine(Arg.Any<DateTime>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void AggregateFlush_WriteOlderEntriesSpecificAndAggregated()
        {
            SetupBufferLineMeasurements();
            Sut.AggregateFlush(SystemDateTime.UtcNow);
            FakeFileHandler.Received(1).WriteLine(Arg.Any<DateTime>(), Arg.Is("M"), Arg.Any<string>());
            FakeFileHandler.Received(3).WriteLine(Arg.Any<DateTime>(), Arg.Is("S"), Arg.Any<string>());
            FakeFileHandler.Received(4).WriteLine(Arg.Any<DateTime>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void AggregateFlushUnconditionally_WriteAllEntriesSpecificAndAggregated()
        {
            SetupBufferLineMeasurements(); Sut.AggregateFlushUnconditionally();
            FakeFileHandler.Received(1).WriteLine(Arg.Any<DateTime>(), Arg.Is("M"), Arg.Any<string>());
            FakeFileHandler.Received(4).WriteLine(Arg.Any<DateTime>(), Arg.Is("S"), Arg.Any<string>());
            FakeFileHandler.Received(5).WriteLine(Arg.Any<DateTime>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void x()
        {
            var x = new BufferLineMeasurement(new DateTime(2929,9,9,9,9,9,DateTimeKind.Local), 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f);
            var logText = x.LogText;

            Assert.StartsWith("2929-09-09T09:09:09.0000000+02:00", logText);
        }
    }
}

