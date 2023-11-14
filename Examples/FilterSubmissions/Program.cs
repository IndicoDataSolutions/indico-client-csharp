using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IndicoV2;
using IndicoV2.Submissions.Models;
using Newtonsoft.Json.Linq;

namespace Examples
{
    /// <summary>
    /// Filters Submissions for all COMPLETE or FAILED submissions that have
    /// not been retrieved yet.
    /// </summary>
    public class FilterSubmissions
    {
        private static string GetToken() =>
            File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "indico_api_token.txt"));

        public static async Task GetCompleteSubmissions(dynamic submissionClient)
        {
            // Filters for just submissions with a COMPLETE status
            var justCompleteFilter = new SubmissionFilter { Status = SubmissionStatus.COMPLETE };
            var submissions = new List<ISubmission> (await submissionClient.ListAsync(null, new[] { 2802 }, justCompleteFilter));
            Console.WriteLine(submissions.Count);
        }

        public static async Task GetCompleteAndFailedSubmissions(dynamic submissionClient)
        {
            // Filters for submissions that have a COMPLETE status and are not retrieved
            var completeFilter = new AndFilter
            {
                And = new List<IFilter>() {
                    new SubmissionFilter { Retrieved = false },
                    new SubmissionFilter { Status = SubmissionStatus.COMPLETE }
                }
            };
            // Filters for submissions that have a FAILED status and are not retrieved
            var failedFilter = new AndFilter
            {
                And = new List<IFilter>() {
                    new SubmissionFilter { Retrieved = false },
                    new SubmissionFilter { Status = SubmissionStatus.FAILED }
                }
            };
            // Filters for submissions that meet the requirements for completeFilter or failedFilter
            var filters = new OrFilter
            {
                Or = new List<IFilter>() { completeFilter, failedFilter }
            };
            var submissions = new List<ISubmission> (await submissionClient.ListAsync(null, new[] { 2802 }, filters));
            Console.WriteLine(submissions.Count);
        }

        public static async Task Main()
        {
            var client = new IndicoClient(GetToken(), new Uri("https://try.indico.io"));

            var submissionClient = client.Submissions();

            await GetCompleteSubmissions(submissionClient);
            await GetCompleteAndFailedSubmissions(submissionClient);
        }
    }
}
