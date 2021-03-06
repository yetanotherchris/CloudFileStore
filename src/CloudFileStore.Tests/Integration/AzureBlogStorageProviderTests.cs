using CloudFileStore.Azure;
using Xunit.Abstractions;

namespace CloudFileStore.Tests.Integration
{
	public class AzureBlogStorageProviderTests : ProviderTestsBase
	{
		private AzureBlobConfiguration _azureBlobConfiguration;

		public AzureBlogStorageProviderTests(ITestOutputHelper outputHelper) : base(outputHelper)
		{
			_azureBlobConfiguration = BindConfiguration<AzureBlobConfiguration>("AzureBlobConfiguration");
			outputHelper.WriteLine($"Azure container: {_azureBlobConfiguration.ContainerName}");
		}

		public override IStorageProvider CreateStorageProvider()
		{
			return new AzureBlobStorageProvider(_azureBlobConfiguration);
		}
	}
}