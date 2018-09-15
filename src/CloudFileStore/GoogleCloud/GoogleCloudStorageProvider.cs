using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Newtonsoft.Json;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace CloudFileStore.GoogleCloud
{
	public class GoogleCloudStorageProvider : IStorageProvider
	{
		private readonly GoogleCloudConfiguration _configuration;
		private readonly StorageClient _storageClient;

		public GoogleCloudStorageProvider(GoogleCloudConfiguration configuration)
		{
			_configuration = configuration;

			string json = JsonConvert.SerializeObject(_configuration);
			GoogleCredential credential = GoogleCredential.FromJson(json);

			_storageClient = StorageClient.Create(credential);
		}

		public async Task<IEnumerable<string>> ListFilesAsync(int pageSize = 100, bool pagingEnabled = true)
		{
			var options = new ListObjectsOptions()
			{
				PageSize = pageSize,
			};

			var objects = await _storageClient.ListObjectsAsync(_configuration.BucketName, "", options)
											  .ReadPageAsync(pageSize);

			return objects.Select(x => x.Name);
		}

		public async Task<string> LoadTextFileAsync(string filename)
		{
			Object item = _storageClient.GetObject(_configuration.BucketName, filename);

			if (item != null)
			{
				using (Stream stream = new MemoryStream())
				{
					_storageClient.DownloadObject(item, stream);
					stream.Position = 0;
					using (var reader = new StreamReader(stream))
					{
						return await reader.ReadToEndAsync();
					}
				}
			}

			return "";
		}

		public async Task SaveTextFileAsync(string filePath, string fileContent, string contentType = "text/plain")
		{
			using (Stream stream = new MemoryStream())
			{
				using (var streamWriter = new StreamWriter(stream))
				{
					await streamWriter.WriteAsync(fileContent);
					streamWriter.Flush();

					await _storageClient.UploadObjectAsync(_configuration.BucketName, filePath, contentType, stream);
				}
			}
		}

		public async Task DeleteFileAsync(string filename)
		{
			await _storageClient.DeleteObjectAsync(_configuration.BucketName, filename);
		}

		public async Task<bool> FileExistsAsync(string filename)
		{
			try
			{
				var storageObject = await _storageClient.GetObjectAsync(_configuration.BucketName, filename);
				if (storageObject != null)
					return true;
			}
			catch (Exception)
			{
			}

			return false;
		}
	}
}