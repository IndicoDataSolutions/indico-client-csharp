namespace Indico.Jobs
{
    public class JobOptions
    {
        public int Priority { get; }

        public JobOptions(int priority = 2)
        {
            this.Priority = priority;
        }
    }
}
