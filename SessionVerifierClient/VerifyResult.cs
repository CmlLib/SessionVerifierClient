using System.Text.Json.Serialization;

namespace SessionVerifierClient;

public record VerifyResult(
    [property:JsonPropertyName("uuid")] string UUID,
    [property:JsonPropertyName("username")] string username);