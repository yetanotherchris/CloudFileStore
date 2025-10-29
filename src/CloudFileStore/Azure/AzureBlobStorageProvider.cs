using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CloudFileStore.Azure
{
    public class AzureBlobStorageProvider : IStorageProvider
    {
        private readonly AzureBlobConfiguration _configuration;
        private readonly CloudBlobContainer _blobContainer;
        private BlobContinuationToken _continuationToken;

        public AzureBlobStorageProvider(AzureBlobConfiguration configuration)
        {
            _configuration = configuration;

            CloudStorageAccount storageAccount;
            CloudStorageAccount.TryParse(configuration.ConnectionString, out storageAccount);

            var client = storageAccount.CreateCloudBlobClient();
            _blobContainer = client.GetContainerReference(configuration.ContainerName);
        }

        public async Task DeleteFileAsync(string filename)
        {
            var blob = _blobContainer.GetBlobReference(filename);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<bool> FileExistsAsync(string filename)
        {
            var blob = _blobContainer.GetBlobReference(filename);
            return await blob.ExistsAsync();
        }

        public async Task<IEnumerable<string>> ListFilesAsync(int pageSize = 100, bool pagingEnabled = true)
        {
            BlobResultSegment segment = null;

            if (pagingEnabled)
            {
                segment = await _blobContainer.ListBlobsSegmentedAsync("", true, BlobListingDetails.All, pageSize, _continuationToken, new BlobRequestOptions(), new OperationContext());
                _continuationToken = segment.ContinuationToken;
            }
            else
            {
                segment = await _blobContainer.ListBlobsSegmentedAsync(null);
            }

            return segment.Results
                          .Select(x => Path.GetFileName(x.Uri.LocalPath));
        }

        public async Task<string> LoadTextFileAsync(string filename)
        {
            CloudBlob blob = _blobContainer.GetBlobReference(filename);

            using (var stream = await blob.OpenReadAsync())
            {
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public async Task SaveTextFileAsync(string filePath, string fileContent, string contentType = "text/plain")
        {
            CloudBlockBlob blob = _blobContainer.GetBlockBlobReference(filePath);
            await blob.UploadTextAsync(fileContent);
        }
    }
}