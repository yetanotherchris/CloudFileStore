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

		public async Task<IEnumerable<string>> ListFilesAsync()
		{
			var objects = await _storageClient.ListObjectsAsync(_configuration.BucketName).ReadPageAsync(100);
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

		public async Task SaveTextFileAsync(string filePath, string fileContent)
		{
			using (Stream stream = new MemoryStream())
			{
				using (var streamWriter = new StreamWriter(stream))
				{
					await streamWriter.WriteAsync(fileContent);
					streamWriter.Flush();

					await _storageClient.UploadObjectAsync(_configuration.BucketName, filePath, null, stream);
				}
			}
		}
	}
}