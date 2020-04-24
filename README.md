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

#### Get a Model Group

Note that the model group ID is found on the model's Review page in the Indico App.
```
ModelGroup mg = client.ModelGroupQuery(mgId).Exec();
```

#### Load a Model
```
// Note that you're passing in a ModelGroup object returned from ModelGroupQuery.
// You get the ModelGroup object through a query and pass the the Model Group ID

ModelGroup mg = client.ModelGroupQuery(mgId).Exec();
String status = client.ModelGroupLoad(mg).Exec();
```

#### Get Model Predictions
```
// Again, passing in a ModelGroup object. Also passing in a List of the text to predict.
// It's always much more efficient to pass in a list to predict. A List of 3 or 3,000 samples
// to predict is fine.

ModelGroup mg = client.ModelGroupQuery(mgId).Exec();
Job job = client.ModelGroupPredict(mg).Data(texts).Exec();

// Retrieve the predictions as a JArray

JArray jobResult = job.Results();
```

#### OCR Documents.

DocumentExtraction is extremely configurable. Five pre-set configurations are provided: `standard`, `legacy`, `simple`, `detailed` and `ondocument`.

Most users will only need to use “standard” to get both document and page-level text and block positions in a nested 
response format (returned object is a nested dictionary).

The “simple” configuration provides a basic and fast (3-5x faster) OCR option for native PDFs- i.e. it will not work 
with scanned documents. Returns document, page, and block-level text and the returned object is a nested dictionary.

The “legacy” configuration is principally intended for users who ran Indico’s original pdf_extraction function to 
extract text and train models. Use “legacy” if you are adding samples to models that were trained with data using 
the original pdf_extraction. It returns a dictionary containing only the extracted text at the document-level.

The “detailed” configuration provides OCR metrics and details down to the character level- it’s a lot of data. In 
addition to document, page, and block-level text, it provides information on the text font/size, confidence level 
for extracted characters, alternative characters (i.e. second most probable character), character position information, 
and more. It returns a nested dictionary.

“ondocument” provides similar information to “detailed” but does not include text/metadata at the document-level. It 
returns a list of dictionaries where each dictionary is page data.

If the pre-set configurations don't suit your use case then you can create custom DocumentExtraction configurations.
All of the available configuration options are described [here](https://indicodatasolutions.github.io/indico-client-python/docextract_settings.html).

```
JObject extractConfig = new JObject()
{
   { "preset_config", "standard" }
};

// OCR a Single File

DocumentExtraction ocr = client.DocumentExtraction(extractConfig);
Job job = ocr.Exec(args[0]);

// OCR a List of files - Currently, no real benefit to doing this.

List<Job> jobs = indico.DocumentExtraction(extractConfig)
                       .Files(List<string>)                       
                       .Exec();
```

#### Fetch a DocumentExtraction Result From Storage
```
string url = (string)job.Result().GetValue("url");
Blob blob = client.RetrieveBlob(url).Exec();
Console.WriteLine(blob.AsJSONObject());
```

#### Get Training Model With Progress
```
ModelGroup mg = client.ModelGroupQuery(mgId).Exec();
JArray trainingStatus = client.TrainingModelWithProgressQuery(mg).Exec();
            
Console.WriteLine(mg.Name);
Console.WriteLine(trainingStatus);                     
```
