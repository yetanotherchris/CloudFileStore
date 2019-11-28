using System;
using System.Linq;
using Amazon;
using Newtonsoft.Json;

namespace CloudFileStore.AWS
{
	public class S3Configuration
	{
		public string BucketName { get; set; }
		public string SecretKey { get; set; }
		public string AccessKey { get; set; }
		
		/// <summary>
		/// An optional token, if using AWS Session authentication.
		/// </summary>
		public string Token { get;set; }
		
		/// <summary>
		/// For example eu-west-1
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