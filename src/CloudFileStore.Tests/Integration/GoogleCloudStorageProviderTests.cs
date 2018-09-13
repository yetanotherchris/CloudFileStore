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
	public class GoogleCloudStorageProviderTests
	{
		private readonly GoogleCloudConfiguration _googleConfiguration;

		public GoogleCloudStorageProviderTests()
		{
			// Get configuration from appsettings.json
			var builder = new ConfigurationBuilder()
				.AddUserSecrets(typeof(S3StorageProviderTests).Assembly, true)
				.AddEnvironmentVariables();

			IConfigurationRoot configuration = builder.Build();
			IConfigurationSection section = configuration.GetSection("GoogleCloudConfiguration");

			_googleConfiguration = new GoogleCloudConfiguration();
			section.Bind(_googleConfiguration);
		}

		[Fact]
		public async Task should_save_text_file_content()
		{
			// given
			var provider = new GoogleCloudStorageProvider(_googleConfiguration);
			string filename = $"{DateTime.UtcNow.Ticks.ToString()}.json";

			// when
			await provider.SaveTextFileAsync(filename, "content here");

			// then
			string content = await provider.LoadTextFileAsync(filename);
			content.ShouldNotBeNullOrWhiteSpace();
		}

		[Fact]
		public async Task should_load_text_file_content()
		{
			// given
			var provider = new GoogleCloudStorageProvider(_googleConfiguration);

			// when
			string json = await provider.LoadTextFileAsync("28DT24.json");

			// then
			json.ShouldNotBeNullOrWhiteSpace();
		}

		[Fact]
		public async Task should_list_files()
		{
			// given
			var provider = new GoogleCloudStorageProvider(_googleConfiguration);

			// when
			IEnumerable<string> files = await provider.ListFilesAsync();

			// then
			files.Count().ShouldBeGreaterThanOrEqualTo(1);
			files.FirstOrDefault(x => x == "28DT24.json").ShouldNotBeNull();
		}
	}
}