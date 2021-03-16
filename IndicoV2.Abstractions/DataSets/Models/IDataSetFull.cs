using IndicoV2.Models.Models;

namespace IndicoV2.DataSets.Models
{
    public interface IDataSetFull : IDataSet
    {
        int NumModelGroups { get; }
        int RowCount { get; }
        string Status { get; }
        IModelGroupBase[] ModelGroups { get; }
    }
}
