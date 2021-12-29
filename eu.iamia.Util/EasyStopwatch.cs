using System.Diagnostics;
// ReSharper disable UnusedMember.Global

namespace eu.iamia.Util
{
    /// <summary>
    /// Easy use of Stopwatch.
    /// 
    /// Starts when instantiated.
    /// Returns duration when stopped.
    /// 
    /// Usage:
    /// * var stopwatch = new EasyStopwatch();
    /// * var duration = stopwatch.Stop();
    /// </summary>
    public class EasyStopwatch
    {
        private Stopwatch Stopwatch { get; }

        private bool ShowMicroSeconds { get; init; }

        /// <summary>
        /// Stop Stopwatch and return duration in micro- or milliseconds.
        /// </summary>
        /// <returns></returns>
        public long Stop()
        {
            Stopwatch.Stop();
            return ShowMicroSeconds
                ? 1000000L * Stopwatch.ElapsedTicks / Stopwatch.Frequency
                : Stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Create and start Stopwatch.
        /// </summary>
        private EasyStopwatch()
        {
            Stopwatch = new();
            Stopwatch.Start();
        }

        public static EasyStopwatch StartMs()
        {
            return new() {ShowMicroSeconds = false};
        }

        /// <summary>
        /// Measure in Micro Seconds.
        /// </summary>
        /// <returns></returns>
        public static EasyStopwatch StartUs()
        {
            return new() {ShowMicroSeconds = true};
        }
    }
}