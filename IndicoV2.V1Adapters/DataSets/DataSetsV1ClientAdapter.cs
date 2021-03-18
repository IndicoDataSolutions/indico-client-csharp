using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indico;
using IndicoV2.DataSets;
using IndicoV2.DataSets.Models;
using IndicoV2.V1Adapters.DataSets.Models;
using Newtonsoft.Json.Linq;

namespace IndicoV2.V1Adapters.DataSets
{
    public class DataSetsV1ClientAdapter : IDataSetClient
    {
        private readonly IndicoClient _indicoClientLegacy;

        public DataSetsV1ClientAdapter(Indico.IndicoClient indicoClientLegacy) => _indicoClientLegacy = indicoClientLegacy;


        public async Task<IEnumerable<IDataSet>> ListAsync(CancellationToken cancellationToken = default)
        {
            string query = @"
              query GetDatasets {
                datasets {
                  id
                  name
                }
              }
            ";

            var request = _indicoClientLegacy.GraphQLRequest(query, "GetDatasets");
            var response = await request.Call();
            var dataSets = ((JArray)response["datasets"]).Select(r => new V1DataSetAdapter(r)).ToArray();

            return dataSets;
        }
    }
}