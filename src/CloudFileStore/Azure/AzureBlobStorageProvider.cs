using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CloudFileStore.Azure
{
    public class AzureBlobStorageProvider : IStorageProvider
    {
        private readonly AzureBlobConfiguration _configuration;
        private readonly BlobContainerClient _blobContainer;
        private string _continuationToken;

        public AzureBlobStorageProvider(AzureBlobConfiguration configuration)
        {
            _configuration = configuration;

            var blobServiceClient = new BlobServiceClient(configuration.ConnectionString);
            _blobContainer = blobServiceClient.GetBlobContainerClient(configuration.ContainerName);
        }

        public async Task DeleteFileAsync(string filename)
        {
            var blobClient = _blobContainer.GetBlobClient(filename);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<bool> FileExistsAsync(string filename)
        {
            var blobClient = _blobContainer.GetBlobClient(filename);
            var response = await blobClient.ExistsAsync();
            return response.Value;
        }

        public async Task<IEnumerable<string>> ListFilesAsync(int pageSize = 100, bool pagingEnabled = true)
        {
            var blobs = _blobContainer.GetBlobsAsync();
            var pages = blobs.AsPages(pagingEnabled ? _continuationToken : null, pageSize);

            var results = new List<string>();
            await foreach (var page in pages)
            {
                _continuationToken = page.ContinuationToken;
                results.AddRange(page.Values.Select(x => x.Name));
                break;
            }

            return results;
        }

        public async Task<string> LoadTextFileAsync(string filename)
        {
            var blobClient = _blobContainer.GetBlobClient(filename);
            var response = await blobClient.DownloadContentAsync();
            return response.Value.Content.ToString();
        }

        public async Task SaveTextFileAsync(string filePath, string fileContent, string contentType = "text/plain")
        {
            var blobClient = _blobContainer.GetBlobClient(filePath);
            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            };
            await blobClient.UploadAsync(new BinaryData(fileContent), options);
        }
    }
}
