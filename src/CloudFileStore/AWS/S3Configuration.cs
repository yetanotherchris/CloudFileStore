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
		public string Region { get; set; } // e.g. eu-west-1

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