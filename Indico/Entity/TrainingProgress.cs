namespace Indico.Entity
{
    public class TrainingProgress
    {
        public float PercentComplete { get; }

        public TrainingProgress(float percentComplete)
        {
            this.PercentComplete = percentComplete;
        }
    }
}
