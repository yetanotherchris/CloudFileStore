using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace CloudFileStore.AWS
{
	public class S3StorageProvider : IStorageProvider
	{
		private readonly S3Configuration _configuration;
		private readonly AmazonS3Client _s3Client;

		public S3StorageProvider(S3Configuration configuration)
		{
			_configuration = configuration;

			var credentials = new BasicAWSCredentials(_configuration.AccessKey, _configuration.SecretKey);
			_s3Client = new AmazonS3Client(credentials, _configuration.RegionEndpoint);
		}

		public async Task<IEnumerable<string>> ListFilesAsync()
		{
			ListObjectsResponse objects = await _s3Client.ListObjectsAsync(_configuration.BucketName);
			return objects.S3Objects.Select(x => x.Key);
		}

		public async Task<string> LoadTextFileAsync(string filename)
		{
			GetObjectResponse response = await _s3Client.GetObjectAsync(new GetObjectRequest
			{
				BucketName = _configuration.BucketName,
				Key = filename
			});

			using (var reader = new StreamReader(response.ResponseStream))
			{
				return await reader.ReadToEndAsync();
			}
		}

		public async Task SaveTextFileAsync(string filePath, string fileContent)
		{
			// S3Client handles the disposing of the Stream
			Stream stream = new MemoryStream();
			var streamWriter = new StreamWriter(stream);
			await streamWriter.WriteAsync(fileContent);
			streamWriter.Flush();

			var putRequest = new PutObjectRequest()
			{
				BucketName = _configuration.BucketName,
				ContentType = "text/plain",
				InputStream = stream,
				Key = filePath
			};

			await _s3Client.PutObjectAsync(putRequest);
		}
	}
}