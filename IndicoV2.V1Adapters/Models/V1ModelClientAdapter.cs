using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Indico.Mutation;
using Indico.Query;
using IndicoV2.Models;
using IndicoV2.Models.Models;
using IndicoV2.V1Adapters.Models.Models;

namespace IndicoV2.V1Adapters.Models
{
    public class V1ModelClientAdapter : IModelClient
    {
        private readonly Indico.IndicoClient _clientLegacy;

        public V1ModelClientAdapter(Indico.IndicoClient indicoClientLegacyClient) => _clientLegacy = indicoClientLegacyClient;

        public async Task<IModelGroup> GetGroup(int modelGroupId, CancellationToken cancellationToken) =>
            new V1ModelGroupAdapter(
                await new ModelGroupQuery(_clientLegacy.GraphQLHttpClient)
                {
                    MgId = modelGroupId
                }.Exec(cancellationToken));

        public Task<string> LoadModel(int modelId, CancellationToken cancellationToken) =>
            new ModelGroupLoad(_clientLegacy.GraphQLHttpClient) {ModelId = modelId}.Exec(cancellationToken);

        public async Task<string> Predict(int modelId, List<string> data, CancellationToken cancellationToken) =>
            (await
                new ModelGroupPredict(_clientLegacy.GraphQLHttpClient) { ModelId = modelId }
                .Data(data)
                .Exec(cancellationToken))
            .Id;
    }
}