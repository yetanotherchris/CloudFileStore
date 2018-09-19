using System.IO;
using CloudFileStore.AWS;
using Microsoft.Extensions.Configuration;

namespace CloudFileStore.Tests.Integration
{
	public class S3StorageProviderTests : ProviderTestsBase
	{
		private S3Configuration _s3Configuration;

		public S3StorageProviderTests()
		{
			_s3Configuration = BindConfiguration<S3Configuration>("S3Configuration");
		}

		public override IStorageProvider CreateStorageProvider()
		{
			return new S3StorageProvider(_s3Configuration);
		}
	}
}