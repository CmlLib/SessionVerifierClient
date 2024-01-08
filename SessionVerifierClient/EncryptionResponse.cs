using System.Text.Json.Serialization;

namespace SessionVerifierClient;

public record EncryptionResponse(
    [property:JsonPropertyName("sharedSecret")] string SharedSecret,
    [property:JsonPropertyName("verifyToken")] string VerifyToken);