using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IndicoV2;
using IndicoV2.Submissions.Models;

namespace Examples
{
    public class SubmissionFilterExample
    {
        private static string GetToken() =>
            File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "indico_api_token.txt"));

        public static async Task Main()
        {
            var filters = new OrFilter
            {
                Or = new List<IFilter>
                {
                    new SubmissionFilter
                    {
                        InputFilename = "",
                        Status = SubmissionStatus.COMPLETE,
                        Retrieved = false
                    },
                    new SubmissionFilter
                    {
                        InputFilename = "",
                        Status = SubmissionStatus.FAILED,
                        Retrieved = false
                    }
                }
            };

            var client = new IndicoClient(GetToken(), new Uri("https://try.indico.io"));

            var submissionClient = client.Submissions();
            var submissionIds = new List<int>() { };
            var workflowIds = new List<int>() { 3106 };

            var submissions = await submissionClient.ListAsync(submissionIds, workflowIds, filters, limit: 1000);
            Console.WriteLine(submissions);
        }
    }
}