using System;
using Indico.Entity;
using IndicoV2.Models.Models;

namespace IndicoV2.V1Adapters.Models.Models
{
    internal class V1ModelAdapter : IModel
    {
        private readonly Model _model;

        public V1ModelAdapter(Model model) => _model = model ?? throw new ArgumentNullException(nameof(model)); 

        public int Id => _model.Id;

        public string Status => _model.Status;

        public float TrainingProgressPercents => _model.TrainingProgress?.PercentComplete ?? 0;
    }
}
