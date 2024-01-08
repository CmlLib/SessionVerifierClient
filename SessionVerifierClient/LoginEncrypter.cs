using System.Security.Cryptography;

namespace SessionVerifierClient;

public class LoginEncrypter
{
    private readonly Yggdrasil _yggdrasil;

    public LoginEncrypter(Yggdrasil yggdrasil)
    {
        _yggdrasil = yggdrasil;
    }

    public async Task<EncryptionResponse> Encrypt(Session session, EncryptionRequest req)
    {
        if (string.IsNullOrEmpty(session.AccessToken))
            throw new ArgumentException("session.AccessToken");
        if (string.IsNullOrEmpty(session.UUID))
            throw new ArgumentException("session.UUID");

        var sharedSecret = new byte[16];
        RandomNumberGenerator.Fill(sharedSecret);

        var publicKeyBytes = Convert.FromBase64String(req.PublicKey);
        await _yggdrasil.Join(session.AccessToken, session.UUID, req.ServerId, sharedSecret, publicKeyBytes);

        using var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        var encryptedSharedSecret = rsa.Encrypt(sharedSecret, RSAEncryptionPadding.Pkcs1);
        var encryptedVerifyToken = rsa.Encrypt(Convert.FromBase64String(req.VerifyToken), RSAEncryptionPadding.Pkcs1);

        return new EncryptionResponse(
            Convert.ToBase64String(encryptedSharedSecret), 
            Convert.ToBase64String(encryptedVerifyToken));
    }
}