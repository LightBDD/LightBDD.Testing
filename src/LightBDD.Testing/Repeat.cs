using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LightBDD.Testing
{
    public class Repeat
    {
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(1);
        public static readonly Func<int, TimeSpan> DefaultIntervals = repeats => TimeSpan.FromMilliseconds(100);
        public TimeSpan Timeout { get; private set; } = DefaultTimeout;
        public Func<int, TimeSpan> Intervals { get; private set; } = DefaultIntervals;

        public static async Task<T> UntilAsync<T>(TimeSpan timeout, Func<T> query, Func<T, bool> successCondition, string timeoutMessage)
            => await UntilAsync(timeout, () => Task.FromResult(query()), successCondition, timeoutMessage);

        public static async Task<T> UntilAsync<T>(TimeSpan timeout, Func<Task<T>> asyncQuery, Func<T, bool> successCondition, string timeoutMessage)
        {
            return await new Repeat()
                .SetTimeout(timeout)
                .SetIntervals(DefaultIntervals)
                .RepeatUntilAsync(asyncQuery, successCondition, timeoutMessage);
        }

        public Repeat SetIntervals(Func<int, TimeSpan> intervals)
        {
            Intervals = intervals;
            return this;
        }

        public Repeat SetInterval(TimeSpan interval) => SetIntervals(repeats => interval);

        public Repeat SetTimeout(TimeSpan timeout)
        {
            Timeout = timeout;
            return this;
        }

        public async Task<T> RepeatUntilAsync<T>(Func<T> query, Func<T, bool> successCondition, string timeoutMessage)
            => await RepeatUntilAsync(() => Task.FromResult(query()), successCondition, timeoutMessage);

        public async Task<T> RepeatUntilAsync<T>(Func<Task<T>> asyncQuery, Func<T, bool> successCondition, string timeoutMessage)
        {
            var watch = Stopwatch.StartNew();
            int repeats = 0;
            while (true)
            {
                var value = await asyncQuery();
                if (successCondition(value))
                    return value;
                if (watch.Elapsed > Timeout)
                    throw new RepeatTimeoutException<T>(timeoutMessage, value);

                await Task.Delay(Intervals(++repeats));
            }
        }
    }
}
