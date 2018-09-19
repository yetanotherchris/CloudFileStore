using CloudFileStore.Azure;

namespace CloudFileStore.Tests.Integration
{
	public class AzureBlogStorageProviderTests : ProviderTestsBase
	{
		private AzureBlobConfiguration _azureBlobConfiguration;

		public AzureBlogStorageProviderTests()
		{
			_azureBlobConfiguration = BindConfiguration<AzureBlobConfiguration>("AzureBlobConfiguration");
		}

		public override IStorageProvider CreateStorageProvider()
		{
			return new AzureBlobStorageProvider(_azureBlobConfiguration);
		}
	}
}