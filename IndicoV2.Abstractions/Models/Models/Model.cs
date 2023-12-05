namespace IndicoV2.Models.Models
{
    public class Model : IModel
    {
        public int Id { get; set; } = 0;
        public string Status { get; set; }
        public float TrainingProgressPercents { get; set; } = 0f;
    }
}
