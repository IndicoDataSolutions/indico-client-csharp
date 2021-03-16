using System;
using Indico.Entity;
using IndicoV2.Models.Models;

namespace IndicoV2.V1Adapters.Models.Models
{
    internal class V1ModelGroupAdapter : IModelGroup
    {
        private readonly ModelGroup _modelGroup;

        public V1ModelGroupAdapter(ModelGroup modelGroup) => _modelGroup = modelGroup;

        public int Id => _modelGroup.Id;
        public string Name => _modelGroup.Name;
        public ModelStatus Status => Map(_modelGroup.Status);
        public IModel SelectedModel => new V1ModelAdapter(_modelGroup.SelectedModel);

        private ModelStatus Map(Indico.Types.ModelStatus modelGroupStatus) =>
            (ModelStatus)Enum.Parse(typeof(ModelStatus), modelGroupStatus.ToString());
    }
}
