using System;
using Indico;
using Indico.Mutation;
using Indico.Jobs;
using Indico.Storage;
using Newtonsoft.Json.Linq;

namespace Examples
{
  class SingleDocExtraction 
  {
    static void Main(string[] args)
    {
      IndicoConfig config = new IndicoConfig(
          host: "dev.indico.io",
          tokenPath: "__TOKEN_DIRECTORY__"
      );
      IndicoClient client = new IndicoClient(config);
      List<string> files = new List<string>()
      {
        "__PDF_PATH__"
      };
      JObject json = new JObject()
      {
        { "preset_config", "simple" }
      };
      DocumentExtraction extraction = indico.DocumentExtraction();
      List<Job> jobs = extraction.Files(files).JsonConfig(json).Execute();
      Job job = jobs[0];
      JObject obj = job.Result().Result;
      string url = (string) obj.GetValue("url");
      RetrieveBlob retrieveBlob = indico.RetrieveBlob();
      Blob blob = retrieveBlob.Url(url).Execute();
      Console.WriteLine(blob.AsJSONObject());
    }
  }
}
