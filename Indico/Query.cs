using System.Threading;
using System.Threading.Tasks;

namespace Indico
{
    internal interface IQuery<T>
    {
        /// <summary>
        /// Execute the graphql query and returns the results as a specific type
        /// </summary>
        /// <returns>result of query of type T</returns>
        Task<T> Exec(CancellationToken cancellationToken = default);
    }
}