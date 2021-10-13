namespace IndicoV2.Workflows.Models
{
    public interface IWorkflow
    {
        /// <summary>
        /// Workflow's Id
        /// </summary>
        int Id { get; }

        bool ReviewEnabled { get; }
    }
}
