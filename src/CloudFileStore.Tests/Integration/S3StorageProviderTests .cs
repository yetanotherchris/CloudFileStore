using System.IO;
using CloudFileStore.AWS;
using Microsoft.Extensions.Configuration;

namespace CloudFileStore.Tests.Integration
{
	public class S3StorageProviderTests : ProviderTestsBase
	{
		private S3Configuration _s3Configuration;

		public override void ReadConfiguration()
		{
			// Get configuration from user secrets:
			// https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=windows
			// - %APPDATA%\Microsoft\UserSecrets\CloudFileStore.Tests\secrets.json
			// - type .\appsettings.development.json | dotnet user-secrets set

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddUserSecrets(typeof(S3StorageProviderTests).Assembly, true)
				.AddEnvironmentVariables();

			IConfigurationRoot configuration = builder.Build();
			IConfigurationSection section = configuration.GetSection("S3Configuration");

			_s3Configuration = new S3Configuration();
			section.Bind(_s3Configuration);
		}

		public override IStorageProvider CreateStorageProvider()
		{
			return new S3StorageProvider(_s3Configuration);
		}
	}
}