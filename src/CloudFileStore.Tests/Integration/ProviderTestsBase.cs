﻿using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CloudFileStore.Tests.Integration
{
	public abstract class ProviderTestsBase
	{
		public abstract void ReadConfiguration();

		public abstract IStorageProvider CreateStorageProvider();

		public ProviderTestsBase()
		{
			ReadConfiguration();
		}

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
	}
}