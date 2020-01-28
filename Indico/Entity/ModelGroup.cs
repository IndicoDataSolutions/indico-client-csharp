using System;
using Indico.Types;
using Newtonsoft.Json.Linq;

namespace Indico.Entity
{
    public class ModelGroup
    {
        /// <summary>
        /// Gets the ModelGroup identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; }
        /// <summary>
        /// Gets the ModelGroup name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
        /// <summary>
        /// Gets the ModelGroup status.
        /// </summary>
        /// <value>The status.</value>
        public ModelStatus Status { get; }
        /// <summary>
        /// Gets the selected model.
        /// </summary>
        /// <value>The selected model.</value>
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