using System.Text.Json.Serialization;

namespace CloudFileStore.GoogleCloud
{
    public class GoogleCloudConfiguration
    {
        public string BucketName { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

#pragma warning disable IDE1006 // Naming Styles
        public string project_id { get; set; }
        public string private_key_id { get; set; }
        public string private_key { get; set; }
        public string client_email { get; set; }
        public string client_id { get; set; }
        public string auth_uri { get; set; }
        public string token_uri { get; set; }
        public string auth_provider_x509_cert_url { get; set; }
        public string client_x509_cert_url { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}