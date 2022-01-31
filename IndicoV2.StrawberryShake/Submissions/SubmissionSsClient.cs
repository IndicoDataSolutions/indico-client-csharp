using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.StrawberryShake.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace IndicoV2.StrawberryShake.Submissions
{
    public class SubmissionSsClient : ErrorHandlingWrapper
    {
        private readonly ServiceProvider _services;

        public SubmissionSsClient(ServiceProvider services) => _services = services;

        public async Task<IEnumerable<int>> Create(int workflowId, IEnumerable<(string Name, string Meta)> files, CancellationToken cancellationToken) => 
            (await ExecuteAsync(async () => await _services.GetRequiredService<WorkflowSubmissionMutation>().ExecuteAsync(
                workflowId, 
                files.Select(f => new FileInput { Filename = f.Name, Filemeta = RemovePropsCausingErrors(f.Meta)}).ToArray(), 
                default,
                cancellationToken)))
            .WorkflowSubmission
            .SubmissionIds
            .Select(id => id.Value);

        public async Task<IListSubmissions_Submissions> List(IReadOnlyList<int?> ids, IReadOnlyList<int?> workflowIds, SubmissionFilter? filter, int? limit, int? after, CancellationToken cancellationToken) => (
            await ExecuteAsync(async () => await _services.GetRequiredService<ListSubmissionsQuery>().ExecuteAsync(ids, workflowIds, filter, limit, null, null, after, cancellationToken))).Submissions;

        public async Task<int?> MarkRetrieved(int submissionId, bool retrieved = true, CancellationToken cancellationToken = default) => (await ExecuteAsync(async() => await _services.GetRequiredService<UpdateSubmissionMutation>().ExecuteAsync(submissionId, retrieved, cancellationToken))).UpdateSubmission.Id;
        private string RemovePropsCausingErrors(string metaString)
        {
            var acceptableMetaProps = new[] { "name", "path", "upload_type" };
            var meta = JsonDocument.Parse(metaString);
            var newProps = meta.RootElement.EnumerateObject().Where(prop => acceptableMetaProps.Contains(prop.Name));

            using var output = new MemoryStream();
            using var outputWriter = new Utf8JsonWriter(output);
            
            outputWriter.WriteStartObject();

            foreach (var prop in newProps)
            {
                prop.WriteTo(outputWriter);
            }

            outputWriter.WriteEndObject();
            outputWriter.Flush();
            
            output.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(output);
            var fixedMetaString = reader.ReadToEnd();

            return fixedMetaString;
        }
    }
}
