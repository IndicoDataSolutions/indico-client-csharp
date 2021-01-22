using System.Threading.Tasks;

namespace Indico
{
    internal interface IRestRequest<T>
    {
        Task<T> Call();
    }
}
