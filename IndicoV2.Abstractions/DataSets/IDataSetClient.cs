using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Abstractions.DataSets.Models;

namespace IndicoV2.Abstractions.DataSets
{
    public interface IDataSetClient
    {
        Task<IEnumerable<DataSet>> ListAsync(CancellationToken cancellationToken = default);
    }
}