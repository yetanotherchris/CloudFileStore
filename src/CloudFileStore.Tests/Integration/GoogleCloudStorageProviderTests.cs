using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudFileStore.GoogleCloud;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Xunit;

namespace CloudFileStore.Tests.Integration
{
	public class GoogleCloudStorageProviderTests : ProviderTestsBase
	{
		private GoogleCloudConfiguration _googleConfiguration;

		public override void ReadConfiguration()
		{
			// Get configuration from appsettings.json
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddUserSecrets(typeof(S3StorageProviderTests).Assembly, true)
				.AddEnvironmentVariables();

			IConfigurationRoot configuration = builder.Build();
			IConfigurationSection section = configuration.GetSection("GoogleCloudConfiguration");

			_googleConfiguration = new GoogleCloudConfiguration();
			section.Bind(_googleConfiguration);
		}

		public override IStorageProvider CreateStorageProvider()
		{
			return new GoogleCloudStorageProvider(_googleConfiguration);
		}
	}
}