using Azure.Storage.Blobs;
using CloudFileStore.Azure;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace CloudFileStore.Tests.Integration
{
    public class AzureBlogStorageProviderTests : ProviderTestsBase
    {
        private AzureBlobConfiguration _azureBlobConfiguration;

        public AzureBlogStorageProviderTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            _azureBlobConfiguration = new AzureBlobConfiguration();

            var configuration = GetConfiguration();
            IConfigurationSection section = configuration.GetSection("AzureBlobConfiguration");
            section.Bind(_azureBlobConfiguration);

            EnsureAzureContainerExists();

            outputHelper.WriteLine($"Azure container: {_azureBlobConfiguration.ContainerName}");
        }

        private void EnsureAzureContainerExists()
        {
            var blobServiceClient = new BlobServiceClient(_azureBlobConfiguration.ConnectionString);
            var container = blobServiceClient.GetBlobContainerClient(_azureBlobConfiguration.ContainerName);
            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        public override IStorageProvider CreateStorageProvider()
        {
            return new AzureBlobStorageProvider(_azureBlobConfiguration);
        }
    }
}
