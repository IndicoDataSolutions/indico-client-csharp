using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.IntegrationTests.Utils.DataHelpers;
using IndicoV2.Storage;
using IndicoV2.Storage.Models;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.Storage
{
    public class StorageClientTests
    {
        private IStorageClient _storageClient;
        private DataHelper _dataHelper;

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _storageClient = container.Resolve<IStorageClient>();
            _dataHelper = container.Resolve<DataHelper>();
        }

        [Test]
        public async Task UploadAsync_ShouldReturnUploadedFileInfo()
        {
            // Arrange
            var filePath = _dataHelper.Files().GetSampleFilePath();
            
            // Act
            var uploadedFiles = (await _storageClient.UploadAsync(new[] {filePath}, default)).ToArray();
            
            // Assert
            uploadedFiles.Should().HaveCount(1);
            var uploadedFile = uploadedFiles.Single();

            uploadedFile.Name.Should().Be(Path.GetFileName(filePath));
            uploadedFile.Path.Should().NotBeEmpty();
            uploadedFile.UploadType.Should().Be(UploadType.User);
        }
    }
}
