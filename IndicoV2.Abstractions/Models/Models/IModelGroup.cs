namespace IndicoV2.Models.Models
{
    public interface IModelGroupBase
    {
        /// <summary>
        /// Gets the ModelGroup identifier.
        /// </summary>
        /// <value>The identifier.</value>
        int Id { get; }
    }

    public interface IModelGroup : IModelGroupBase
    {
        /// <summary>
        /// Gets the ModelGroup name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the ModelGroup status.
        /// </summary>
        /// <value>The status.</value>
        string Status { get; }

        /// <summary>
        /// Gets the selected model.
        /// </summary>
        /// <value>The selected model.</value>
        IModel SelectedModel { get; }
    }
}
