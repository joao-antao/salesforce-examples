using System.Text.Json.Serialization;

namespace Salesforce;

public sealed class SalesforceAccessToken
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
    
    [JsonPropertyName("instance_url")]
    public required string InstanceUrl { get; set; }
}