using System;
using Xunit;

namespace eu.iamia.Util.UnitTest
{
    public class SystemDateTimeShouldWhenStopped
    {
        private DateTime Initial;

        private void Init(int hour)
        {
            Initial = new DateTime(2021, 01, 01, hour, 30, 0, DateTimeKind.Utc);
            SystemDateTime.SetTime(Initial, 0d);
        }

        [Theory]
        [InlineData(00)]
        [InlineData(23)]
        public void UtcNow_ReturnInitial(int hour)
        {
            Init(hour);
            Assert.Equal(Initial, SystemDateTime.UtcNow);
        }

        [Theory]
        [InlineData(00)]
        [InlineData(23)]
        public void Today_ReturnsInitialDate(int hour)
        {
            Init(hour);
            Assert.Equal(Initial.ToLocalTime().Date, SystemDateTime.Today);
        }

        [Theory]
        [InlineData(00)]
        [InlineData(23)]
        public void Tomorrow_ReturnInitialDatePlusOne(int hour)
        {
            Init(hour);
            Assert.Equal(Initial.AddDays(1).ToLocalTime().Date, SystemDateTime.Tomorrow);
        }

        [Theory]
        [InlineData(00)]
        [InlineData(23)]
        public void Yesterday_ReturnInitialDateMinusOne(int hour)
        {
            Init(hour);
            Assert.Equal(Initial.AddDays(-1).ToLocalTime().Date, SystemDateTime.Yesterday);
        }

        [Theory]
        [InlineData(00)]
        [InlineData(23)]
        public void Now_ReturnInitial(int hour)
        {
            Init(hour);
            Assert.Equal(Initial.ToLocalTime(), SystemDateTime.Now);
        }
    }
}
