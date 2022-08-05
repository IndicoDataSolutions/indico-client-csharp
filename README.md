# indico-client-csharp

## Installation

Install the indico-client-csharp package via the NuGet package manager GUI or command line.
```
PM> Install-Package indico-client
```

## Authentication

The Indico Platform and Client Libraries use JSON Web Tokens (JWT) for user authentication. You can download a token 
from your [user dashboard](https://app.indico.io/auth/account) by clicking the large, blue “Download new API Token” button. 
Most browsers will download the API token as indico_api_token.txt and place it in your Downloads directory. You should move 
the token file from Downloads to either your user profile `(C:\Users\my-user)` or another location in your development 
environment. The C# Client Library will look in your user profile directory by default.

## Configuration

### IndicoConfig Class
The IndicoConfig class gives you the maximum control over C# Client Library configuration. Here’s how you might instantiate 
an IndicoConfig object and set the host:
```
IndicoConfig config = new IndicoConfig(host: "app.indico.io");
```
You will most often provide a `host` argument to IndicoConfig. The class constructor also accepts tokenPath (path to a token file),
apiToken (the actual token string) and protocol (https).

## API Client

The Indico Platform uses GraphQL to communicate with ALL clients including the company’s own web application and also the 
Indico Python Client. You’ll use an IndicoClient object to pass GraphQL queries to the Indico Platform. Here’s a simple way 
to create a client:
```
IndicoConfig config = new IndicoConfig(host: "app.indico.io");

IndicoClient indico = new IndicoClient(config);
```
The default config for IndicoClient sets the host to `app.indico.io` and will look for the Token File in your user profile directory.

If you want to learn more about GraphQL, the [How to GraphQL](https://www.howtographql.com/) tutorial is a great place to start. 



## Indico GraphQL Schema

The Indico Platform ships with a built-in sandbox environment that both documents and allows you to interactively explore 
the Platform’s GraphQL schema. You can find the sandbox at `/graph/api/graphql` on your Indico Platform installation. If your 
Platform’s host is `indico.my_company.com` then the full sandbox URL would be `https://indico.my_company.com/graph/api/graphql`.

## Pre-Built GraphQL Queries

GraphQL is extremely powerful, flexible and efficient but can be a bit verbose. To make things easier for day-to-day use of the 
Platform and Client Library, the developers at Indico created a collection of Python Classes to generate the most often used 
queries for you. You can find the collection documented in the Reference section of the Client Libreary Docs.

## Examples

Several examples are provided in this repo:

**GraphqlCall** - Place a GraphQL call to list your datasets
**SingleDocExtraction** - OCR a single PDF file (a sample PDF is provided)
**GetPredictions** - Get predictions from a simple classifier. A CSV is provided for you to train the classifier via the Indico App.
**GetModelTrainingProgress** - Get % complete progress on a training model.
**OcrPredict** - Pass a directory of files through DocumentExtraction then pass the text to a model for predictions.

The examples are setup as console apps in the repo's Visual Studio project.

## Example Snippets

#### Create a Client
```
IndicoConfig config = new IndicoConfig(host: "app.indico.io");
IndicoClient indico = new IndicoClient(config);
```

#### Submit a Document to a Workflow and wait for result
var submissionClient = client.Submissions();
var submissionIds = await submissionClient.CreateAsync(workflow_id, new[] {"workflow-sample.pdf"});
int submissionId = submissionIds.Single();
var submission = await submissionClient.GetAsync(submissionId);
var jobResult = await client.GetSubmissionResultAwaiter().WaitReady(submissionId);
```

```

#### Get Training Model With Progress
```
ModelGroup mg = await client.ModelGroupQuery(mgId).Exec();
JArray trainingStatus = await client.TrainingModelWithProgressQuery(mg).Exec();
            
Console.WriteLine(mg.Name);
Console.WriteLine(trainingStatus);                     
```

#### Send a GraphQL Query

As noted above, the Indico Platform presents a GrapQL interface. In addition
to using this Client Library, you can also send GraphQL queries to the Platform.
Here's a snippet to list all of the Datasets in your Indico account.

```
string query = @"
    query GetDatasets {
        datasets {
            id
            name
            status
            rowCount
            numModelGroups
            modelGroups {
                id
            }
        }
      }
    ";

    GraphQLRequest request = client.GraphQLRequest(query, "GetDatasets");            
    JObject response = await request.Call();
    Console.WriteLine(response);
```