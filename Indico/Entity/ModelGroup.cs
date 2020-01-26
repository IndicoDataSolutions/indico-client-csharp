using System;
using Indico.Types;
using Newtonsoft.Json.Linq;

namespace Indico.Entity
{
    public class ModelGroup
    {
        public int Id { get; }
        public string Name { get; }
        public ModelStatus Status { get; }
        public Model SelectedModel { get; }

        internal ModelGroup(JObject mg)
        {
            this.Id = (int)mg.GetValue("id");
            this.Name = (string)mg.GetValue("name");
            string status = (string)mg.GetValue("status");
            this.Status = (ModelStatus)Enum.Parse(typeof(ModelStatus), status);
            JObject selectedModel = (JObject)mg.GetValue("selectedModel");
            this.SelectedModel = new Model(selectedModel);
        }
    }
}