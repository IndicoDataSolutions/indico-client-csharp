using System.Linq;
using System.Threading.Tasks;
using IndicoV2.DataSets;
using IndicoV2.DataSets.Models;

namespace IndicoV2.IntegrationTests.Utils.DataHelpers.DataSets
{
    public class DataSetHelper
    {
        private readonly IDataSetClient _dataSetClient;

        public DataSetHelper(IDataSetClient dataSetClient) => _dataSetClient = dataSetClient;
        public async Task<IDataSet> GetAny() => (await _dataSetClient.ListFullAsync(1)).Single();
    }
}
