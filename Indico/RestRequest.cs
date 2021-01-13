using System.Threading.Tasks;

namespace Indico
{
    internal interface RestRequest<T>
    {
        Task<T> Call();
    }
}
