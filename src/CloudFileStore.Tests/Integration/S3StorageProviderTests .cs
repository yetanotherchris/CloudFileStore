using System;
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
			// Get configuration from user secrets:
			// https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=windows
			// - %APPDATA%\Microsoft\UserSecrets\CloudFileStore.Tests\secrets.json
			// - type .\appsettings.development.json | dotnet user-secrets set

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddUserSecrets(typeof(S3StorageProviderTests).Assembly, true)
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
			var provider = new S3StorageProvider(_s3Configuration);
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
			var provider = new S3StorageProvider(_s3Configuration);

			// when
			IEnumerable<string> firstPageOfFiles = await provider.ListFilesAsync();
			IEnumerable<string> secondPageOfFiles = await provider.ListFilesAsync();

			// then
			firstPageOfFiles.Count().ShouldBeGreaterThanOrEqualTo(1);
			secondPageOfFiles.Count().ShouldBeGreaterThanOrEqualTo(1);

			firstPageOfFiles.ShouldNotBeSameAs(secondPageOfFiles);
		}

		[Fact]
		public async Task should_check_file_exists()
		{
			// given
			var provider = new S3StorageProvider(_s3Configuration);
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
			var provider = new S3StorageProvider(_s3Configuration);

			// when
			bool exists = await provider.FileExistsAsync("this file doesn't exist");

			// then
			exists.ShouldBeFalse();
		}

		[Fact]
		public async Task should_delete_file()
		{
			// given
			var provider = new S3StorageProvider(_s3Configuration);
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