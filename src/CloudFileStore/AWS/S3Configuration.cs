using System;
using System.Linq;
using Amazon;
using Newtonsoft.Json;

namespace CloudFileStore.AWS
{
	public class S3Configuration
	{
		public string BucketName { get; set; }
		
		/// <summary>
		/// The user's access key for S3 authentication. Leave blank for ~/.aws/credentials based authentication.
		/// </summary>
		public string AccessKey { get; set; }
		
		/// <summary>
		/// The user's secret key for S3 authentication. Leave blank for ~/.aws/credentials based authentication.
		/// </summary>
		public string SecretKey { get; set; }
		
		/// <summary>
		/// The users' optional session token, if using AWS Session authentication.
		/// </summary>
		public string Token { get;set; }
		
		/// <summary>
		/// The AWS region, for example eu-west-1. Leave blank to use the ~/.aws/ config value.
		/// </summary>
		public string Region { get; set; }

		[JsonIgnore]
		public RegionEndpoint RegionEndpoint
		{
			get
			{
				return RegionEndpoint.EnumerableAllRegions.First(x =>
					x.SystemName.Equals(Region, StringComparison.InvariantCultureIgnoreCase));
			}
		}
	}
}