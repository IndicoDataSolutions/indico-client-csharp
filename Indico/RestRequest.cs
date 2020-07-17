using System.Threading.Tasks;

namespace Indico
{
    interface RestRequest<T>
    {
        Task<T> Call();
    }
}
