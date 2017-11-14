using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LutronCaseta
{
    public static class ObservableExtensions
    {
        public static Task<T> WaitFor<T>(this IObservable<T> source, Func<T, bool> pred, int timeoutMs = 1000, CancellationToken token = default(CancellationToken))
        {
            T returnValue = default(T);
            try
            {
                return
                    source
                        .Where(pred)
                        .Timeout(TimeSpan.FromMilliseconds(timeoutMs))
                        .DistinctUntilChanged()
                        .Take(1)
                        .ToTask(token);
            }
            catch
            {
                return Task.FromResult(returnValue);
            }
        }
    }
}
