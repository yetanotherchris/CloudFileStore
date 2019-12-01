using System.IO;
using CloudFileStore.AWS;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace CloudFileStore.Tests.Integration
{
	public class S3StorageProviderTests : ProviderTestsBase
	{
		private S3Configuration _s3Configuration;

		public S3StorageProviderTests(ITestOutputHelper outputHelper)
		{
			_s3Configuration = BindConfiguration<S3Configuration>("S3Configuration");
			outputHelper.WriteLine($"AWS bucket name: ${_s3Configuration.BucketName}");
		}

		public override IStorageProvider CreateStorageProvider()
		{
			return new S3StorageProvider(_s3Configuration);
		}
	}
}