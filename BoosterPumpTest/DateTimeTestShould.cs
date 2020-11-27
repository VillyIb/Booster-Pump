using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System.Diagnostics.CodeAnalysis;

namespace BoosterPumpTest
{
    public class DateTimeComparer : EqualityComparer<DateTime>
    {
        public override bool Equals([AllowNull] DateTime x, [AllowNull] DateTime y)
        {
            return Math.Abs(x.Ticks - y.Ticks) < TimeSpan.TicksPerMillisecond;
        }

        public override int GetHashCode([DisallowNull] DateTime obj)
        {
            return (obj.Ticks / TimeSpan.TicksPerMillisecond).GetHashCode();
        }
    }

    public class DateTimeTestShould
    {
        [Fact]
        public void UnstartedReturnsActualTime()
        {
            DateTimeTest.Start(null);
            Assert.Equal(DateTime.Now, DateTimeTest.Now, new DateTimeComparer());
        }

        [Fact]
        public void StartedReturnsSpecifiedTimeUtc()
        {
            var startTime = new DateTime(2000, 2, 29, 01, 35, 48, DateTimeKind.Utc);
            DateTimeTest.Start(startTime);

            var virtualTime = DateTimeTest.UtcNow;
            Assert.Equal(startTime, virtualTime, new DateTimeComparer());
        }

        [Fact]
        public void StartedReturnsSpecifiedTimeLocal()
        {
            var startTime = new DateTime(2000, 2, 29, 02, 35, 48, DateTimeKind.Utc);
            DateTimeTest.Start(startTime);

            var expectedLocalTime = startTime.ToLocalTime();
            var virtualTime = DateTimeTest.Now;
            Assert.Equal(expectedLocalTime, virtualTime, new DateTimeComparer());
        }

    }
}
