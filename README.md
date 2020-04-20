# indico-client-csharp

## Authentication

The Indico Platform and Client Libraries use JSON Web Tokens (JWT) for user authentication. You can download a token from your [user dashboard](https://app.indico.io/auth/account) by clicking the large, blue “Download new API Token” button. Most browsers will download the API token as indico_api_token.txt and place it in your Downloads directory. You should move the token file from Downloads to either your user profile (C:\Users\my-user) or another location in your development environment. The C# Client Library will look in your user profile directory by default.

## Configuration

### IndicoConfig Class
The IndicoConfig class gives you the maximum control over C# Client Library configuration. Here’s how you might instantiate an IndicoConfig object and set the host and token path:

## API Client

The Indico Platform uses GraphQL to communicate with ALL clients including the company’s own web application and also the Indico Python Client. You’ll use an IndicoClient object to pass GraphQL queries to the Indico Platform. Here’s a simple way to create a client:
```
IndicoClient indico = new IndicoClient(IndicoConfig config);
```
The IndicoClient constructor will read configuation options from the environment variables described above. If you would like to manually set configuration options in an IndicoConfig object then you can pass your config to IndicoClient as follows:

client = IndicoClient(config=my_config)

If you want to learn more about GraphQL, the [How to GraphQL](https://www.howtographql.com/) tutorial is a great place to start.

## Indico GraphQL Schema

The Indico Platform ships with a built-in sandbox environment that both documents and allows you to interactively explore the Platform’s GraphQL schema. You can find the sandbox at /graph/api/graphql on your Indico Platform installation. If your Platform’s host is `indico.my_company.com` then the full sandbox URL would be `https://indico.my_company.com/graph/api/graphql`.

## Pre-Built GraphQL Queries

GraphQL is extremely powerful, flexible and efficient but can be a bit verbose. To make things easier for day-to-day use of the Platform and Client Library, the developers at Indico created a collection of Python Classes to generate the most often used queries for you. You can find the collection documented in the Reference section of the Client Libreary Docs.

### Example Snippets

```

// Create Client
IndicoClient indico = new IndicoClient(IndicoConfig config);

// Get Model Group
ModelGroup mg = indico.ModelGroupQuery()
                      .Id(int id)
                      .Query();

// Get Training Model With Progress
ModelGroup mg = indico.TrainingModelWithProgressQuery()
                      .Id(int id)
                      .Query();

// Load Model
String status = indico.ModelGroupLoad()
                      .ModelGroup(mg)
                      .Execute();

// To Upload Files
JArray metadata = indico.UploadFile()
                        .FilePaths(List<string>)
                        .Call();

// Predict Data
Job job = indico.ModelGroupPredict()
                .ModelGroup(mg)
                .Data(List<string>)
                .Execute();
JArray jobResult = await job.Results();

// To Extract Documents
List<Job> jobs = indico.DocumentExtraction()
                       .Files(List<string>)
                       .JsonConfig(JObject)
                       .Execute();

// To Fetch Result From Storage
Blob blob = indico.RetrieveBlob()
                  .Url(string)
                  .Execute();
```
