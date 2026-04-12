using System;
using System.Linq;
using FluentAssertions;
using IndicoV2.Storage;
using IndicoV2.Storage.Models;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace IndicoV2.Tests.Storage
{
    /// <summary>
    /// Unit tests validating C# SDK storage classes are compatible with the
    /// storage-service response shapes (the Rainbow replacement).
    ///
    /// Covered:
    ///   - RetrieveBlob.Url: indico-file:// path extraction
    ///   - StorageClient.DeserializeMetadata: LegacyUploadResponseItem → FileMetadata
    ///   - JToken name/meta extraction pattern used in StorageClient.UploadAsync
    /// </summary>
    [TestFixture]
    public class StorageCompatTests
    {
        /// <summary>
        /// JSON array produced by storage-service /files/store endpoint.
        /// Mirrors LegacyUploadResponseItem in storage_service/routes/blob_routes.py.
        /// </summary>
        private const string StorageServiceUploadJson = @"[
            {
                ""path"": ""/uploads/42/abc-uuid"",
                ""name"": ""document.pdf"",
                ""size"": 12345,
                ""upload_type"": ""user""
            }
        ]";

        private static readonly Uri TestBaseUri = new Uri("https://test.example.com");
        private static IndicoClient FakeClient => new IndicoClient("fake-token", TestBaseUri);

        // ---------------------------------------------------------------------------
        // RetrieveBlob — indico-file:// URI handling
        // ---------------------------------------------------------------------------

        [Test]
        public void RetrieveBlob_Url_ExtractsPath_FromIndicoFileSchemeUri()
        {
            // storage-service produces indico-file:///storage/<path> URIs.
            // Note: Uri.ToString() adds a trailing slash to the root so the
            // concatenation produces a double slash (//storage/...). This is
            // pre-existing behavior that nginx normalises silently.
            var blob = new RetrieveBlob(FakeClient)
            {
                Url = "indico-file:///storage/submissions/1/2/result.json"
            };

            blob.Url.Should().Be("https://test.example.com//storage/submissions/1/2/result.json");
        }

        [Test]
        public void RetrieveBlob_Url_ExtractsPath_FromHttpUri()
        {
            // Handles plain https URIs (e.g. pre-signed MinIO URLs returned by legacy flows).
            // Double-slash is pre-existing; storage-service nginx normalises it.
            var blob = new RetrieveBlob(FakeClient)
            {
                Url = "https://test.example.com/storage/extractions/99.json"
            };

            blob.Url.Should().Be("https://test.example.com//storage/extractions/99.json");
        }

        [Test]
        public void RetrieveBlob_Url_StripsSurroundingQuotes()
        {
            // GraphQL can return URI values with surrounding quotes; setter strips them.
            var blob = new RetrieveBlob(FakeClient)
            {
                Url = "\"indico-file:///storage/files/output.json\""
            };

            blob.Url.Should().Be("https://test.example.com//storage/files/output.json");
        }

        // ---------------------------------------------------------------------------
        // StorageClient.DeserializeMetadata — LegacyUploadResponseItem shape
        // ---------------------------------------------------------------------------

        [Test]
        public void DeserializeMetadata_MapsPathAndName_FromStorageServiceResponse()
        {
            var storageClient = new StorageClient(FakeClient);
            var jArray = JArray.Parse(StorageServiceUploadJson);

            var result = storageClient.DeserializeMetadata(jArray).Single();

            result.Path.Should().Be("/uploads/42/abc-uuid");
            result.Name.Should().Be("document.pdf");
        }

        [Test]
        public void DeserializeMetadata_MapsSize_FromStorageServiceResponse()
        {
            var storageClient = new StorageClient(FakeClient);
            var jArray = JArray.Parse(StorageServiceUploadJson);

            var result = (FileMetadata)storageClient.DeserializeMetadata(jArray).Single();

            result.Size.Should().Be(12345);
        }

        [Test]
        public void DeserializeMetadata_MapsUploadType_UserStringToEnum()
        {
            // storage-service returns "upload_type": "user" (lowercase string).
            // Newtonsoft.Json enum deserialization must match it to UploadType.User.
            var storageClient = new StorageClient(FakeClient);
            var jArray = JArray.Parse(StorageServiceUploadJson);

            var result = storageClient.DeserializeMetadata(jArray).Single();

            result.UploadType.Should().Be(UploadType.User);
        }

        [Test]
        public void DeserializeMetadata_HandlesMultipleFiles()
        {
            var json = @"[
                {""path"":""/uploads/1/uuid-a"",""name"":""a.pdf"",""size"":100,""upload_type"":""user""},
                {""path"":""/uploads/1/uuid-b"",""name"":""b.pdf"",""size"":200,""upload_type"":""user""}
            ]";
            var storageClient = new StorageClient(FakeClient);
            var results = storageClient.DeserializeMetadata(JArray.Parse(json)).ToList();

            results.Should().HaveCount(2);
            results[0].Name.Should().Be("a.pdf");
            results[1].Name.Should().Be("b.pdf");
        }

        // ---------------------------------------------------------------------------
        // JToken name/meta extraction (mirrors StorageClient.UploadAsync stream path)
        // ---------------------------------------------------------------------------

        [Test]
        public void UploadAsync_NameMetaExtraction_ReadsNameFromStorageServiceItem()
        {
            // StorageClient.UploadAsync reads t.Value<string>("name") and t.ToString()
            // as the (Name, Meta) tuple. Verify this pattern works on storage-service JSON.
            var jArray = JArray.Parse(StorageServiceUploadJson);

            var (name, meta) = (
                jArray[0].Value<string>("name"),
                jArray[0].ToString()
            );

            name.Should().Be("document.pdf");
            meta.Should().Contain("/uploads/42/abc-uuid");
            meta.Should().Contain("upload_type");
        }
    }
}
