using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Models.Models;

namespace IndicoV2.Models
{
    public interface IModelClient
    {
        /// <summary>
        /// Gets <seealso cref="IModelGroup"/>
        /// </summary>
        /// <param name="modelGroupId"><seealso cref="IModelGroup"/>'s Id</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns></returns>
        Task<IModelGroup> GetGroup(int modelGroupId, CancellationToken cancellationToken);
    }
}
