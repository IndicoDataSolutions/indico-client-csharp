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
        /// Gets the Model info.
        /// </summary>
        /// <value>The model info.</value>
        public JObject ModelInfo { get; }

        public Model(JObject model)
        {
            this.Id = (int)model.GetValue("id");
            string modelInfo = (string)model.GetValue("modelInfo");
            this.ModelInfo = JsonConvert.DeserializeObject<JObject>(modelInfo);
        }
    }
}