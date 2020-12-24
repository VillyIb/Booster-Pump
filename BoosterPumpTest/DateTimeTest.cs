using System;
namespace BoosterPumpTest
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Use SystemDateTime", true)]
    public class DateTimeTest
    {
        /// <summary>
        /// Correction to real DateTime.
        /// </summary>
        private static long OffSett { get; set; }

        public static void Start(DateTime? startTimeUtc = null)
        {
            OffSett = startTimeUtc == null ? 0L : startTimeUtc.Value.Ticks - DateTime.UtcNow.Ticks;
        }

        static DateTimeTest()
        {
            OffSett = 0L;
        }

        public static DateTime Now => new DateTime(DateTime.Now.Ticks + OffSett, DateTimeKind.Local);

        public static DateTime UtcNow => new DateTime(DateTime.UtcNow.Ticks + OffSett, DateTimeKind.Utc);
    }
}
