# indico-client-csharp

### How To Use

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
