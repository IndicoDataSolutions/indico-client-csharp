namespace IndicoV2.Models.Models
{
    public class ModelGroup : IModelGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ModelStatus Status { get; set; }
        public IModel SelectedModel { get; set; }
    }
}
