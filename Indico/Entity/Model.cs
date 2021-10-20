using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace Indico.Entity
{
    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    public class Model
    {
        /// <summary>
        /// Gets the Model identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
        /// <summary>
        /// Gets the Model status.
        /// </summary>
        /// <value>The model status.</value>
        public string Status { get; set; }
        /// <summary>
        /// Gets training progress.
        /// </summary>
        /// <value>The training progress.</value>
        public TrainingProgress TrainingProgress { get; set; }
    }
}