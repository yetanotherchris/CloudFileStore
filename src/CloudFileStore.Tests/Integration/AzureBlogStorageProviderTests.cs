using CloudFileStore.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
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

            // Ensure test Azure container exists for tests 
            EnsureAzureContainerExists();

            outputHelper.WriteLine($"Azure container: {_azureBlobConfiguration.ContainerName}");
        }

        private void EnsureAzureContainerExists()
        {
            CloudStorageAccount storageAccount;
            CloudStorageAccount.TryParse(_azureBlobConfiguration.ConnectionString, out storageAccount);

            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(_azureBlobConfiguration.ContainerName);
            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        public override IStorageProvider CreateStorageProvider()
        {
            return new AzureBlobStorageProvider(_azureBlobConfiguration);
        }
    }
}