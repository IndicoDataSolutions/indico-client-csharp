using System.Threading;
using System.Threading.Tasks;

namespace IndicoV2
{
    internal interface IRestRequest<T>
    {
        Task<T> Call(CancellationToken cancellationToken = default);
    }
}
