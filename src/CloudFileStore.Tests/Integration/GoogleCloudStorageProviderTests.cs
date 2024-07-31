using CloudFileStore.GoogleCloud;
using System.IO;
using System.Text.Json;
using Xunit.Abstractions;

namespace CloudFileStore.Tests.Integration
{
    public class GoogleCloudStorageProviderTests : ProviderTestsBase
	{
		private GoogleCloudConfiguration _googleConfiguration;

		public GoogleCloudStorageProviderTests(ITestOutputHelper outputHelper) : base(outputHelper)
		{
            var appsettingsConfig = BindConfiguration<GoogleCloudConfiguration>("GoogleCloudConfiguration");

			if (File.Exists("google.json"))
			{
				// For Github Actions, as writing to a single file is easier than manipulating appsettings
				string googleJson = File.ReadAllText("google.json");
				var googleConfig = JsonSerializer.Deserialize<GoogleCloudConfiguration>(googleJson);
				outputHelper.WriteLine(googleConfig.Type);

				_googleConfiguration = googleConfig;
				_googleConfiguration.BucketName = appsettingsConfig.BucketName;
			}
			else
			{
                _googleConfiguration = appsettingsConfig;
            }

            outputHelper.WriteLine($"Google bucket: {_googleConfiguration.BucketName}");
        }

		public override IStorageProvider CreateStorageProvider()
		{
			return new GoogleCloudStorageProvider(_googleConfiguration);
		}
	}
}