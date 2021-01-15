using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.DataSets.Models;

namespace IndicoV2.DataSets
{
    public interface IDataSetClient
    {
        Task<IEnumerable<IDataSet>> ListAsync(CancellationToken cancellationToken = default);
    }
}