using System.Threading;
using System.Threading.Tasks;
using Indico;
using Indico.Query;
using IndicoV2.Models;
using IndicoV2.Models.Models;
using IndicoV2.V1Adapters.Models.Models;

namespace IndicoV2.V1Adapters.Models
{
    public class V1ModelClientAdapter : IModelClient
    {
        private readonly IndicoClient _clientLegacy;

        public V1ModelClientAdapter(IndicoClient indicoClientLegacyClient) => _clientLegacy = indicoClientLegacyClient;

        public async Task<IModelGroup> GetGroup(int modelGroupId, CancellationToken cancellationToken) =>
            new V1ModelGroupAdapter(
                await new ModelGroupQuery(_clientLegacy.GraphQLHttpClient)
                {
                    MgId = modelGroupId
                }.Exec(cancellationToken));
    }
}