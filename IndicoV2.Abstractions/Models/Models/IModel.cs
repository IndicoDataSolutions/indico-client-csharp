namespace IndicoV2.Models.Models
{
    public interface IModel
    {
        /// <summary>
        /// Gets the Model identifier.
        /// </summary>
        /// <value>The identifier.</value>
        
        int Id { get; }
        /// <summary>
        /// Gets the Model status.
        /// </summary>
        /// <value>The model status.</value>
        
        string Status { get; }

        /// <summary>
        /// Gets training progress.
        /// </summary>
        /// <value>The training progress.</value>
        float TrainingProgressPercents { get; }
    }
}
