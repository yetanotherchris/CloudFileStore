using CloudFileStore.GoogleCloud;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace CloudFileStore.Tests.Integration
{
    public class GoogleCloudStorageProviderTests : ProviderTestsBase
    {
        private GoogleCloudConfiguration _googleConfiguration;

        public GoogleCloudStorageProviderTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            var configuration = GetConfiguration();
            _googleConfiguration = new GoogleCloudConfiguration();
            configuration.Bind(_googleConfiguration);
            _googleConfiguration.BucketName = "cloudfilestore-tests"; // should match the Google Cloud Storage bucket name.

            outputHelper.WriteLine($"Google bucket: {_googleConfiguration.BucketName}");
            outputHelper.WriteLine($"Google private_key length: {_googleConfiguration.private_key.Length}");
        }

        public override IStorageProvider CreateStorageProvider()
        {
            return new GoogleCloudStorageProvider(_googleConfiguration);
        }
    }
}