using System;
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
		private string _continuationToken;

		public S3StorageProvider(S3Configuration configuration)
		{
			_configuration = configuration;

			var credentials = new BasicAWSCredentials(_configuration.AccessKey, _configuration.SecretKey);
			_s3Client = new AmazonS3Client(credentials, _configuration.RegionEndpoint);
		}

		public async Task<IEnumerable<string>> ListFilesAsync(int pageSize = 100, bool pagingEnabled = true)
		{
			var request = new ListObjectsV2Request()
			{
				MaxKeys = pageSize,
				BucketName = _configuration.BucketName,
			};

			if (pagingEnabled)
				request.ContinuationToken = _continuationToken;

			ListObjectsV2Response response = await _s3Client.ListObjectsV2Async(request);
			_continuationToken = response.NextContinuationToken;

			return response.S3Objects.Select(x => x.Key);
		}

		public async Task<string> LoadTextFileAsync(string filename)
		{
			var request = new GetObjectRequest
			{
				BucketName = _configuration.BucketName,
				Key = filename
			};
			GetObjectResponse response = await _s3Client.GetObjectAsync(request);

			if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
				return null;

			using (var reader = new StreamReader(response.ResponseStream))
			{
				return await reader.ReadToEndAsync();
			}
		}

		public async Task SaveTextFileAsync(string filePath, string fileContent, string contentType = "text/plain")
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

		public async Task DeleteFileAsync(string filename)
		{
			var request = new DeleteObjectRequest()
			{
				BucketName = _configuration.BucketName,
				Key = filename
			};

			await _s3Client.DeleteObjectAsync(request);
		}

		public async Task<bool> FileExistsAsync(string filename)
		{
			var request = new GetObjectRequest
			{
				BucketName = _configuration.BucketName,
				Key = filename
			};

			try
			{
				GetObjectResponse response = await _s3Client.GetObjectAsync(request);

				if (response != null || response.HttpStatusCode == System.Net.HttpStatusCode.OK)
					return true;
			}
			catch (Exception)
			{
			}

			return false;
		}
	}
}