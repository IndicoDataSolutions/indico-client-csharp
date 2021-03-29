using System.Collections.Generic;
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

        /// <summary>
        /// Loads Model
        /// </summary>
        /// <param name="modelId"><seealso cref="IModel"/>'s Id</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns>Status</returns>
        Task<string> LoadModel(int modelId, CancellationToken cancellationToken);

        /// <summary>
        /// Predicts on a Model Group.
        /// </summary>
        /// <param name="modelId"><see cref="IModel"/>'s Id</param>
        /// <param name="data">Data to predict</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns>Job's Id</returns>
        Task<string> Predict(int modelId, List<string> data, CancellationToken cancellationToken);
    }
}
