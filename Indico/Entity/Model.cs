using Newtonsoft.Json;
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

        public Model(JObject model)
        {
            this.Id = (int)model.GetValue("id");
            this.Status = (string)model.GetValue("status");
            JToken trainingProgress = model.GetValue("trainingProgress");
            if(trainingProgress != null)
            {
                float percentComplete = trainingProgress.Value<float>("percentComplete");
                this.TrainingProgress = new TrainingProgress(percentComplete);
            }
            else
            {
                this.TrainingProgress = null;
            }
        }
    }
}