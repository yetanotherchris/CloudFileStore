using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudFileStore
{
	public interface IStorageProvider
	{
		Task<IEnumerable<string>> ListFilesAsync();

		Task<string> LoadTextFileAsync(string filename);

		Task SaveTextFileAsync(string filePath, string fileContent);
	}
}