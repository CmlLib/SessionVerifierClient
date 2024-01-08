using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SessionVerifierClient;

public class Yggdrasil
{
    public const string DefaultHost = "https://sessionserver.mojang.com";

    private readonly HttpClient _httpClient;
    private readonly string _host;

    public Yggdrasil(HttpClient httpClient) : this(DefaultHost, httpClient)
    {

    }

    public Yggdrasil(string host, HttpClient httpClient) => 
        (_host, _httpClient) = (host, httpClient);

    public async Task Join(string accessToken, string uuid, string serverId, byte[] sharedSecret, byte[] publicKey)
    {
        using var res = await _httpClient.PostAsJsonAsync($"{_host}/session/minecraft/join", new
        {
            accessToken,
            selectedProfile = uuid.Replace("-", ""),
            serverId = HashServerId(serverId, sharedSecret, publicKey)
        });

        if (!res.IsSuccessStatusCode)
        {
            using var resStream = await res.Content.ReadAsStreamAsync();

            var ex = parseException(resStream);
            if (ex != null)
                throw ex;
            else
                throw new YggdrasilException($"The server returned error with status code {(int)res.StatusCode}");
        }
    }

    public string HashServerId(string serverId, byte[] sharedSecret, byte[] publicKey)
    {
        using var buffer = new MemoryStream();
        buffer.Write(Encoding.ASCII.GetBytes(serverId));
        buffer.Write(sharedSecret);
        buffer.Write(publicKey);
        buffer.Flush();
        var serverIdBytes = buffer.ToArray();

        using var sha1 = SHA1.Create();
        var serverIdHash = sha1.ComputeHash(serverIdBytes, 0, serverIdBytes.Length);
        return HashUtil.ToMinecraftSHA1String(serverIdHash);
    }

    private Exception? parseException(Stream stream)
    {
        try
        {
            using var jsonDoc = JsonDocument.Parse(stream);
            var error = jsonDoc.RootElement.GetProperty("error").GetString();

            if (string.IsNullOrEmpty(error))
                return null;
            else
                return new YggdrasilException(error);
        }
        catch
        {
            return null;
        }
    }
}