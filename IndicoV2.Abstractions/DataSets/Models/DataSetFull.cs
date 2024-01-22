using IndicoV2.Models.Models;

namespace IndicoV2.DataSets.Models
{
    public class DataSetFull : IDataSetFull
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public int NumModelGroups { get; set; }
        public int RowCount { get; set; }
        public string Status { get; set; }
        public IModelGroupBase[] ModelGroups { get; set; }
    }
}
