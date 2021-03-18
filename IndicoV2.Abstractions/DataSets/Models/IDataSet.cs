namespace IndicoV2.DataSets.Models
{
    public interface IDataSet
    {
        /// <summary>
        /// DataSet's Id
        /// </summary>
        int Id { get; }
        string Name { get; }
    }
}
