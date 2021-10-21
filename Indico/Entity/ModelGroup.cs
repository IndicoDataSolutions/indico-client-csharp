using System;
using Indico.Types;

namespace Indico.Entity
{
    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    public class ModelGroup
    {
        /// <summary>
        /// Gets the ModelGroup identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
        /// <summary>
        /// Gets the ModelGroup name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets the ModelGroup status.
        /// </summary>
        /// <value>The status.</value>
        public ModelStatus Status { get; set; }
        /// <summary>
        /// Gets the selected model.
        /// </summary>
        /// <value>The selected model.</value>
        public Model SelectedModel { get; set; }
    }
}