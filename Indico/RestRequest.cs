using System.Threading;
using System.Threading.Tasks;

namespace Indico
{
    internal interface IRestRequest<T>
    {
        Task<T> Call(CancellationToken cancellationToken = default);
    }
}
