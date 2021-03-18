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

        public DataSetsV1ClientAdapter(IndicoClient indicoClientLegacy) => _indicoClientLegacy = indicoClientLegacy;


        public async Task<IEnumerable<IDataSet>> ListAsync(CancellationToken cancellationToken)
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
            var response = await request.Call(cancellationToken);
            var dataSets = ((JArray)response["datasets"]).Select(ds => (IDataSet)ds.ToObject(typeof(V1DataSetAdapter))).ToArray();

            return dataSets;
        }

        public async Task<IEnumerable<IDataSetFull>> ListFullAsync(int? limit = null, CancellationToken cancellationToken = default)
        {
            var query = @"
            query GetDatasets($limit: Int) {
                datasetsPage(limit: $limit) {
                    datasets {
                        id
                        name
                        status
                        rowCount
                        numModelGroups
                        modelGroups {
                            id
                        }
                    }
                }
            }
            ";

            var request = _indicoClientLegacy.GraphQLRequest(query, "GetDatasets");
            request.Variables = new {limit};
            var response = await request.Call(cancellationToken);
            var dataSets = ((JArray)response["datasetsPage"]["datasets"]).Select(ds => (IDataSetFull)ds.ToObject(typeof(V1DataSetFullAdapter))).ToArray();

            return dataSets;
        }
    }
}