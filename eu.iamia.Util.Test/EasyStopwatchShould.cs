namespace eu.iamia.Util.UnitTest
{
    using System.Threading;
    using Xunit;

    public class EasyStopwatchShould
    {
        [Fact]
        public void ReturnDurationInMsOnStop()
        {
            var sut = EasyStopwatch.StartMs();
            Thread.Sleep(10);
            var duration = sut.Stop();
            Assert.True(duration > 10);
        }

        [Fact]
        public void ReturnDurationInUsOnStop()
        {
            var sut = EasyStopwatch.StartUs();
            Thread.Sleep(10);
            var duration = sut.Stop();
            Assert.True(duration > 10000);
        }
    }
}
