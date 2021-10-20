using System;
using System.Threading;
using System.Threading.Tasks;

namespace Indico
{

    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    internal interface IMutation<T>
    {
        /// <summary>
        /// Execute the graphql query and returns the results as a specific type
        /// </summary>
        /// <returns>result of query of type T</returns>
        Task<T> Exec(CancellationToken cancellationToken = default);
    }
}