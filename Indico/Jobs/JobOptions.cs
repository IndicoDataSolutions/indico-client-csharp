namespace Indico.Jobs
{
    /// <summary>
    /// Configuration parameters to modify how async jobs are handled
    /// </summary>
    public class JobOptions
    {
        /// <summary>
        /// Gets the job priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority { get; }

        public JobOptions(int priority = 2)
        {
            this.Priority = priority;
        }
    }
}
