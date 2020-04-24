using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace Indico.Entity
{
    public class Model
    {
        /// <summary>
        /// Gets the Model identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; }
        /// <summary>
        /// Gets the Model status.
        /// </summary>
        /// <value>The model status.</value>
        public string Status { get; }

        public TrainingProgress TrainingProgress { get; }

        public Model(int id, string status, [Optional] TrainingProgress trainingProgress)
        {
            this.Id = id;
            this.Status = status;
            this.TrainingProgress = trainingProgress;
        }
    }
}