using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudFileStore.GoogleCloud;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace CloudFileStore.Tests.Integration
{
	public class GoogleCloudStorageProviderTests : ProviderTestsBase
	{
		private GoogleCloudConfiguration _googleConfiguration;

		public GoogleCloudStorageProviderTests(ITestOutputHelper outputHelper)
		{
			_googleConfiguration = BindConfiguration<GoogleCloudConfiguration>("GoogleCloudConfiguration");
			outputHelper.WriteLine($"Google bucket: ${_googleConfiguration.BucketName}");
		}

		public override IStorageProvider CreateStorageProvider()
		{
			return new GoogleCloudStorageProvider(_googleConfiguration);
		}
	}
}