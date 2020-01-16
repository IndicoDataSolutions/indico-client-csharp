# indico-client-csharp

### How To Use

```

// Create Client
IndicoClient indico = new IndicoClient(IndicoConfig config);

// Get Model Group
ModelGroup mg = indico.ModelGroupQuery()
                      .Id(int id)
                      .Query();
// Load Model
String status = indico.ModelGroupLoad()
                      .ModelGroup(mg)
                      .Execute();
// Predict Data
Job job = indico.ModelGroupPredict()
                .ModelGroup(mg)
                .Data(List<string>)
                .Execute();
while(job.Status() == JobStatus.PENDING) {
    Thread.Sleep(1000);
}
JArray jobResult = job.Results();

// For Pdf Extraction
Job job = indico.PdfExtraction()
                .Data(List<string>)
                .PdfExtractionOptions(PdfExtractionOptions)
                .Execute();
while(job.Status() == JobStatus.PENDING) {
    Thread.Sleep(1000);
}
JArray jobResult = job.Results();
```