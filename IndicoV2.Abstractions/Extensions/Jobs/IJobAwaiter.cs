using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace IndicoV2.Extensions.Jobs
{
    public interface IJobAwaiter
    {
        Task<TResult> WaitReadyAsync<TResult>(string jobId, TimeSpan checkInterval, CancellationToken cancellationToken = default);

        [Obsolete("Use generic version of this method.")]
        Task<JToken> WaitReadyAsync(string jobId, TimeSpan checkInterval, CancellationToken cancellationToken = default);
    }
}