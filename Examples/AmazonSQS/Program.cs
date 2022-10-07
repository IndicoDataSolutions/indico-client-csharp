using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using IndicoV2;
using Microsoft.Extensions.Configuration;

namespace Examples
{
    public class Program
    {
        private const string _pdfLocation = "workflow-sample.pdf";

        private static async Task Main()
        {
            var conf = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>()
                .Build();

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(15)).Token;

            //configure an indico client
            var indicoClient = new IndicoClient(conf["INDICO_TOKEN"], new Uri(conf["INDICO_HOST"]));

            await Task.WhenAll(
                SubmitWorkflow(indicoClient, int.Parse(conf["INDICO_WORKFLOW_ID"]), cancellationToken),
                HandleSqsMessages(conf, indicoClient, cancellationToken));

            Console.WriteLine("Stopping...");
        }

        private static async Task HandleSqsMessages(IConfigurationRoot conf, IndicoClient indicoClient, CancellationToken cancellationToken)
        {
            //todo: configure your access accordingly before running
            //see: https://docs.aws.amazon.com/sdk-for-java/latest/developerx-guide/credentials.html
            var sqsClient = new AmazonSQSClient(
                new BasicAWSCredentials(conf["AWSAccessKey"], conf["AWSSecretKey"]),
                new AmazonSQSConfig { RegionEndpoint = RegionEndpoint.GetBySystemName(conf["AWSRegion"]) });

            var queueUrl = conf["AWSQueueUrl"];
            var request = new ReceiveMessageRequest(queueUrl)
            {
                MaxNumberOfMessages = 5,
                VisibilityTimeout = 30,
            };

            //request the messages
            while (!cancellationToken.IsCancellationRequested)
            {
                var messages = await sqsClient.ReceiveMessageAsync(request, cancellationToken);

                // iterate the messages from SQS
                foreach (var m in messages.Messages)
                {
                    //fetch receipt handle and body
                    var rh = m.ReceiptHandle;
                    var body = JsonDocument.Parse(m.Body).RootElement;

                    var notificationString = body.GetProperty("Message").GetString();

                    if (notificationString.StartsWith("\""))
                    {
                        // Unescape json
                        notificationString = notificationString
                            .Trim('"')
                            .Replace("\\\"", "\"");
                    }
                    var notification = JsonDocument.Parse(notificationString).RootElement;
                    var status = notification.GetProperty("status").GetString();
                    var url = new Uri(notification.GetProperty("result_url").GetString());
                    var id = notification.GetProperty("submission_id").GetInt32();

                    Console.WriteLine(body);
                    if (status == "COMPLETE")
                    {
                        Console.WriteLine("Complete: " + id);
                        //process a completed message
                        await ProcessResult(indicoClient, url, cancellationToken);
                    }
                    else
                    {
                        //handle failures as seen fit.
                        Console.WriteLine($"Submission {id} failed with status: {status}");
                    }

                    //remove message when processed..
                    var delMsg = new DeleteMessageRequest(queueUrl, rh);
                    await sqsClient.DeleteMessageAsync(delMsg, cancellationToken);
                }

                if (messages.Messages.Count == 0)
                {
                    Console.WriteLine("No new messages, going to sleep.");
                    await Task.Delay(3000, cancellationToken);
                }
            }
        }

        public static async Task ProcessResult(IndicoClient indicoClient, Uri url, CancellationToken cancellationToken)
        {
            Console.WriteLine("get blob....");
            await indicoClient.Storage().GetAsync(url, cancellationToken);
        }

        private static async Task SubmitWorkflow(IndicoClient indicoClient, int workflowId, CancellationToken cancellationToken)
        {
            //todo: change this to your workflow id
            var files = new List<string> { _pdfLocation };
            var submissionIds = await indicoClient.Submissions().CreateAsync(workflowId, files, cancellationToken);
            Console.WriteLine($"Created submission: {submissionIds.Single()}");
        }
    }
}
