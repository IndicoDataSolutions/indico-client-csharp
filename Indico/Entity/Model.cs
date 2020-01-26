using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Indico.Entity
{
    public class Model
    {
        public int Id { get; }
        public JObject ModelInfo { get; }

        internal Model(JObject model)
        {
            this.Id = (int)model.GetValue("id");
            string modelInfo = (string)model.GetValue("modelInfo");
            this.ModelInfo = JsonConvert.DeserializeObject<JObject>(modelInfo);
        }
    }
}