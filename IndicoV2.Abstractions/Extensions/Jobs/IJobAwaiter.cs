using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace IndicoV2.Extensions.Jobs
{
    public interface IJobAwaiter
    {
        Task<JToken> WaitReadyAsync(string jobId, TimeSpan checkInterval, CancellationToken cancellationToken = default);
    }
}