using Microsoft.Extensions.Configuration;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CloudFileStore.Tests.Integration
{
	public abstract class ProviderTestsBase : IDisposable
	{
		protected readonly ITestOutputHelper _outputHelper;

		public ProviderTestsBase(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}
		
		public T BindConfiguration<T>(string sectionName) where T : new()
		{
			// Get configuration from user secrets:
			// https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=windows
			// - %APPDATA%\Microsoft\UserSecrets\CloudFileStore.Tests\secrets.json
			// - .\appsettings.development.json | dotnet user-secrets set

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddUserSecrets(typeof(ProviderTestsBase).Assembly, true)
				.AddEnvironmentVariables();

			IConfigurationRoot configuration = builder.Build();
			IConfigurationSection section = configuration.GetSection(sectionName);

			T instance = new T();
			section.Bind(instance);

			return instance;
		}

		public abstract IStorageProvider CreateStorageProvider();

		protected async Task CreateTestFile(string filename)
		{
			IStorageProvider provider = CreateStorageProvider();
			await provider.SaveTextFileAsync(filename, "content here");
		}

		[Fact]
		public async Task should_save_text_file_content()
		{
			// given
			IStorageProvider provider = CreateStorageProvider();
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
			IStorageProvider provider = CreateStorageProvider();
			string filename = $"{DateTime.UtcNow.Ticks.ToString()}.json";
			await CreateTestFile(filename);

			// when
			string content = await provider.LoadTextFileAsync(filename);

			// then
			content.ShouldNotBeNullOrWhiteSpace();
		}

		[Fact]
		public async Task should_list_files()
		{
			// given
			IStorageProvider provider = CreateStorageProvider();
			for (int i = 0; i < 10; i++)
			{
				string filename = $"{DateTime.UtcNow.Ticks.ToString()}.json";
				await CreateTestFile(filename);
			}

			// when
			IEnumerable<string> firstPageOfFiles = await provider.ListFilesAsync(7);
			IEnumerable<string> secondPageOfFiles = await provider.ListFilesAsync(3);

			// then
			firstPageOfFiles.Count().ShouldBeGreaterThanOrEqualTo(1);
			secondPageOfFiles.Count().ShouldBeGreaterThanOrEqualTo(1);

			firstPageOfFiles.First().ShouldNotBe(secondPageOfFiles.First());
		}

		[Fact]
		public async Task should_list_files_without_pagination()
		{
			// given
			IStorageProvider provider = CreateStorageProvider();
			for (int i = 0; i < 10; i++)
			{
				string filename = $"{DateTime.UtcNow.Ticks.ToString()}.json";
				await CreateTestFile(filename);
			}

			// when
			IEnumerable<string> firstPageOfFiles = await provider.ListFilesAsync(5, false);
			IEnumerable<string> secondPageOfFiles = await provider.ListFilesAsync(5, false);

			// then
			firstPageOfFiles.Count().ShouldBeGreaterThanOrEqualTo(1);
			secondPageOfFiles.Count().ShouldBeGreaterThanOrEqualTo(1);

			firstPageOfFiles.First().ShouldBe(secondPageOfFiles.First());
		}

		[Fact]
		public async Task should_check_file_exists()
		{
			// given
			IStorageProvider provider = CreateStorageProvider();
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
			IStorageProvider provider = CreateStorageProvider();

			// when
			bool exists = await provider.FileExistsAsync("this file doesn't exist");

			// then
			exists.ShouldBeFalse();
		}

		[Fact]
		public async Task should_delete_file()
		{
			// given
			IStorageProvider provider = CreateStorageProvider();
			string filename = $"{DateTime.UtcNow.Ticks.ToString()}.json";
			await provider.SaveTextFileAsync(filename, "content here");

			// when
			await provider.DeleteFileAsync(filename);

			// then
			bool exists = await provider.FileExistsAsync(filename);
			exists.ShouldBeFalse();
		}

		public void Dispose()
		{
			Task cleanupTask = Task.Run(async () =>
			{
				try
				{
					// Remove everything from the bucket once we're finished
					var provider = CreateStorageProvider();
					var files = await provider.ListFilesAsync(100, false);
					foreach (string filename in files)
					{
						try
						{
							await provider.DeleteFileAsync(filename);
						}
						catch (Exception e)
						{
							_outputHelper.WriteLine($"Dispose: failed to delete {filename} - {e.Message}");
						}
					}
				}
				catch (Exception e)
				{
					_outputHelper.WriteLine($"Dispose: failed to cleanup. {e.Message}");
				}
			});

			Task.WaitAll(new Task[] {cleanupTask}, TimeSpan.FromSeconds(5));
		}
	}
}