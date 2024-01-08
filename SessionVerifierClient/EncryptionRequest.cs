using System.Text.Json.Serialization;

namespace SessionVerifierClient;

public record EncryptionRequest(
    [property:JsonPropertyName("serverId")] string ServerId, 
    [property:JsonPropertyName("publicKey")] string PublicKey, 
    [property:JsonPropertyName("verifyToken")] string VerifyToken);
