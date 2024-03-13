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
            var client = new IndicoClient(GetToken(), new Uri("https://try.indico.io"));

            var submissionClient = client.Submissions();
            var submissionIds = new List<int>() { };
            var workflowIds = new List<int>() { 3106 };

            // Example 1
            // List all submissions that are COMPLETE and not retrieved
            var andFilter = new AndFilter
            {
                And = new List<IFilter>
                {
                    new SubmissionFilter
                    {
                        Status = SubmissionStatus.COMPLETE,
                    },
                    new SubmissionFilter
                    {
                        Retrieved = false,
                    }
                }
            };

            submissions = await submissionClient.ListAsync(submissionIds, workflowIds, andFilter, limit: 10);
            Console.WriteLine(submissions);

            // Example 2
            // List all submissions that are complete and not retrieved or failed and not retrieved
            var orFilter = new OrFilter
            {
                Or = new List<IFilter>
                {
                    new AndFilter
                    {
                        And = new List<IFilter>
                        {
                            new SubmissionFilter
                            {
                                Status = SubmissionStatus.COMPLETE,
                            },
                            new SubmissionFilter
                            {
                                Retrieved = false,
                            }
                        }
                    },
                    new AndFilter
                    {
                        And = new List<IFilter>
                        {
                            new SubmissionFilter
                            {
                                Status = SubmissionStatus.FAILED,
                            },
                            new SubmissionFilter
                            {
                                Retrieved = false,
                            }
                        }
                    }
                }
            };

            var submissions = await submissionClient.ListAsync(submissionIds, workflowIds, orFilter, limit: 10);
            Console.WriteLine(submissions);

            // Example 3
            // List all submissions that are retrieved and have a filename that contains 'property'
            var subFilter = new AndFilter
            {
                And = new List<IFilter>
                {
                    new SubmissionFilter
                    {
                        Retrieved = true,
                    },
                    new SubmissionFilter
                    {
                        InputFilename = "property",
                    }
                }
            };

            submissions = await submissionClient.ListAsync(submissionIds, workflowIds, subFilter, limit: 10);
            Console.WriteLine(submissions);

            // Example 4
            // List all submissions that are created and updated within a certain date range
            var dateRangeFilter = new DateRangeFilter()
            {
                From = "2022-01-01",
                To = DateTime.Now.ToString("yyyy-MM-dd")
            };
            subFilter = new AndFilter
            {
                And = new List<IFilter>
                {
                    new SubmissionFilter
                    {
                        CreatedAt = dateRangeFilter,
                    },
                    new SubmissionFilter
                    {
                        UpdatedAt = dateRangeFilter,
                    },
                }
            };

            submissions = await submissionClient.ListAsync(submissionIds, workflowIds, subFilter, limit: 10);
            Console.WriteLine(submissions);

            // Example 5
            // List all submissions that are not in progress of being reviewed and are completed
            subFilter = new AndFilter
            {
                And = new List<IFilter>
                {
                    new SubmissionFilter
                    {
                        Status = SubmissionStatus.COMPLETE,
                    },
                    new SubmissionFilter
                    {
                        ReviewInProgress = false,
                    }
                }
            };

            submissions = await submissionClient.ListAsync(submissionIds, workflowIds, subFilter, limit: 10);
            Console.WriteLine(submissions);
        }
    }
}