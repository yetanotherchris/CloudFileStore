using CloudFileStore.AWS;
using Xunit.Abstractions;

namespace CloudFileStore.Tests.Integration
{
	public class S3StorageProviderTests : ProviderTestsBase
	{
		private S3Configuration _s3Configuration;

		public S3StorageProviderTests(ITestOutputHelper outputHelper)
		{
			outputHelper.WriteLine("Trying to bind S3");
			_s3Configuration = BindConfiguration<S3Configuration>("S3Configuration");
			outputHelper.WriteLine($"AWS bucket name: ${_s3Configuration.BucketName}");
		}

		public override IStorageProvider CreateStorageProvider()
		{
			return new S3StorageProvider(_s3Configuration);
		}
	}
}