using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudFileStore.AWS;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Xunit;

namespace CloudFileStore.Tests.Integration
{
	public class S3StorageProviderTests
	{
		private readonly S3Configuration _s3Configuration;

		public S3StorageProviderTests()
		{
			// Get configuration from appsettings.json
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.development.json", optional: true)
				.AddEnvironmentVariables();

			IConfigurationRoot configuration = builder.Build();
			IConfigurationSection section = configuration.GetSection("S3Configuration");

			_s3Configuration = new S3Configuration();
			section.Bind(_s3Configuration);
		}

		[Fact]
		public async Task should_save_text_file_content()
		{
			// given
			var provider = new S3StorageProvider(_s3Configuration);

			// when
			await provider.SaveTextFileAsync("foo.json", "content here");

			// then
			string json = await provider.LoadTextFileAsync("foo.json");
			json.ShouldNotBeNullOrWhiteSpace();
		}

		[Fact]
		public async Task should_load_text_file_content()
		{
			// given
			var provider = new S3StorageProvider(_s3Configuration);

			// when
			string json = await provider.LoadTextFileAsync("28DT24.json");

			// then
			json.ShouldNotBeNullOrWhiteSpace();
		}

		[Fact]
		public async Task should_list_files()
		{
			// given
			var provider = new S3StorageProvider(_s3Configuration);

			// when
			IEnumerable<string> files = await provider.ListFilesAsync();

			// then
			files.Count().ShouldBeGreaterThanOrEqualTo(1);
			files.FirstOrDefault(x => x == "28DT24.json").ShouldNotBeNull();
		}
	}
}