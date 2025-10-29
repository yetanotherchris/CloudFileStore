using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudFileStore
{
    public interface IStorageProvider
    {
        Task<IEnumerable<string>> ListFilesAsync(int pageSize = 100, bool pagingEnabled = true);

        Task<string> LoadTextFileAsync(string filename);

        Task SaveTextFileAsync(string filePath, string fileContent, string contentType = "text/plain");

        Task DeleteFileAsync(string filename);

        Task<bool> FileExistsAsync(string filename);
    }

    // TODO:
    // - Prefixes for filenames
    // - Graceful error handling
    // - Binary load, save
    // - Update?
    // - Batching?
}