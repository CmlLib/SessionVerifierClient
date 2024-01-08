using System.Text.Json.Serialization;

namespace SessionVerifierClient;

public class Session
{
    [JsonPropertyName("accessToken")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("uuid")]
    public string? UUID { get; set;  }

    [JsonPropertyName("username")]
    public string? Username { get; set; }
}