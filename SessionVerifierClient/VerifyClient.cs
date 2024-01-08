using System.Net.Http.Json;
using System.Text.Json;

namespace SessionVerifierClient;

public class VerifyClient
{
    private readonly HttpClient _httpClient;
    private readonly string _host;

    public VerifyClient(string host, HttpClient httpClient) =>
        (_host, _httpClient) = (host, httpClient);

    public async Task<EncryptionRequest> StartLogin()
    {
        using var res = await _httpClient.GetAsync($"{_host}/startlogin");
        return await handleResponse<EncryptionRequest>(res);
    }

    public async Task<VerifyResult> VerifySecret(string username, string sharedSecret)
    {
        using var res = await _httpClient.PostAsJsonAsync($"{_host}/verifysecret", new
        {
            username,
            sharedSecret
        });
        return await handleResponse<VerifyResult>(res);
    }

    public async Task<VerifyResult> VerifyToken(Session session)
    {
        using var res = await _httpClient.PostAsJsonAsync($"{_host}/verifytoken", session);
        return await handleResponse<VerifyResult>(res);
    }

    private async Task<T> handleResponse<T>(HttpResponseMessage res)
    {
        using var resStream = await res.Content.ReadAsStreamAsync();

        if (res.IsSuccessStatusCode)
        {
            return await JsonSerializer.DeserializeAsync<T>(resStream) ??
                throw new VerifyException("The server returned empty response");
        }
        else
        {
            throw parseException(resStream) ??
                new VerifyException($"The server returned error with status code {(int)res.StatusCode}");
        }
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
                return new VerifyException(error);
        }
        catch
        {
            return null;
        }
    }
}