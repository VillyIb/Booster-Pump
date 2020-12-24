using System;
using Xunit;

namespace eu.iamia.Util.Test
{
    public class SystemDateTimeShould
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
        public void UtcNowReturnInitial(int hour)
        {
            Init(hour);
            Assert.Equal(Initial, SystemDateTime.UtcNow);
        }

        [Theory]
        [InlineData(00)]
        [InlineData(23)]
        public void TodayReturnsInitialDate(int hour)
        {
            Init(hour);
            Assert.Equal(Initial.ToLocalTime().Date, SystemDateTime.Today);
        }

        [Theory]
        [InlineData(00)]
        [InlineData(23)]
        public void TomorrowReturnsInitialDatePlusOne(int hour)
        {
            Init(hour);
            Assert.Equal(Initial.AddDays(1).ToLocalTime().Date, SystemDateTime.Tomorrow);
        }

        [Theory]
        [InlineData(00)]
        [InlineData(23)]
        public void YesterdayReturnsInitialDateMinusOne(int hour)
        {
            Init(hour);
            Assert.Equal(Initial.AddDays(-1).ToLocalTime().Date, SystemDateTime.Yesterday);
        }

        [Theory]
        [InlineData(00)]
        [InlineData(23)]
        public void NowReturnsInitial(int hour)
        {
            Init(hour);
            Assert.Equal(Initial.ToLocalTime(), SystemDateTime.Now);
        }
    }
}
