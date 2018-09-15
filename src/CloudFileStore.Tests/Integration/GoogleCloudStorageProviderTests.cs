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
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
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
			string filename = $"{DateTime.UtcNow.Ticks.ToString()}.json";
			await provider.SaveTextFileAsync(filename, "content here");

			// when
			string content = await provider.LoadTextFileAsync(filename);

			// then
			content.ShouldNotBeNullOrWhiteSpace();
		}

		[Fact]
		public async Task should_list_files()
		{
			// given
			var provider = new GoogleCloudStorageProvider(_googleConfiguration);

			// when
			IEnumerable<string> files = await provider.ListFilesAsync(1);
			IEnumerable<string> files2 = await provider.ListFilesAsync(1);

			// then
			files.Count().ShouldBeGreaterThanOrEqualTo(1);
			files.FirstOrDefault(x => x == "28DT24.json").ShouldNotBeNull();
		}

		[Fact]
		public async Task should_check_file_exists()
		{
			// given
			var provider = new GoogleCloudStorageProvider(_googleConfiguration);
			string filename = $"{DateTime.UtcNow.Ticks.ToString()}.json";
			await provider.SaveTextFileAsync(filename, "content here");

			// when
			bool exists = await provider.FileExistsAsync(filename);

			// then
			exists.ShouldBeTrue();
		}

		[Fact]
		public async Task should_handle_missing_file()
		{
			// given
			var provider = new GoogleCloudStorageProvider(_googleConfiguration);

			// when
			bool exists = await provider.FileExistsAsync("this file doesn't exist");

			// then
			exists.ShouldBeFalse();
		}

		[Fact]
		public async Task should_delete_file()
		{
			// given
			var provider = new GoogleCloudStorageProvider(_googleConfiguration);
			string filename = $"{DateTime.UtcNow.Ticks.ToString()}.json";
			await provider.SaveTextFileAsync(filename, "content here");

			// when
			await provider.DeleteFileAsync(filename);

			// then
			bool exists = await provider.FileExistsAsync(filename);
			exists.ShouldBeFalse();
		}
	}
}