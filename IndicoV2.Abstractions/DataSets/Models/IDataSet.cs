namespace IndicoV2.DataSets.Models
{
    public interface IDataSet
    {
        /// <summary>
        /// DataSet's Id
        /// </summary>
        int Id { get; }

        /// <summary>
        /// DataSet's Name
        /// </summary>
        string Name { get; }
    }
}
