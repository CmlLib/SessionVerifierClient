using SessionVerifierClient;
using System.Text.Json;

var httpClient = new HttpClient();
var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true
};

var session = new Session();

Console.Write("AccessToken: ");
session.AccessToken = Console.ReadLine();
Console.Write("UUID: ");
session.UUID = Console.ReadLine();
Console.Write("Username: ");
session.Username = Console.ReadLine();

//session.AccessToken = "",
//session.Username = "NotedMermaid991";
//session.UUID = "de38931406294f2b8d3025d5d1be88fb";

var host = Environment.GetEnvironmentVariable("SESSION_VERIFIER_HOST");
if (string.IsNullOrEmpty(host))
    host = "http://localhost:23333";

Console.WriteLine("Initializing VerifyClient: " + host);
var verifyClient = new VerifyClient(host, httpClient);
var yggdrasil = new Yggdrasil(httpClient);
var enc = new LoginEncrypter(yggdrasil);

Console.WriteLine("Select login mode: ");
Console.WriteLine("[1] Verifying the secret");
Console.WriteLine("[2] Verifying the token");
var loginMode = Console.ReadLine();

if (loginMode == "1")
{
    Console.WriteLine("StartLogin()");
    var encReq = await verifyClient.StartLogin();
    Console.WriteLine(JsonSerializer.Serialize(encReq, jsonOptions));

    Console.WriteLine("Encrypt()");
    var encRes = await enc.Encrypt(session, encReq);
    Console.WriteLine(JsonSerializer.Serialize(encRes, jsonOptions));

    Console.WriteLine($"VerifySecret(\"{session.Username}\", encRes.SharedSecret)");
    var profile = await verifyClient.VerifySecret(session.Username, encRes.SharedSecret);
    Console.WriteLine(JsonSerializer.Serialize(profile, jsonOptions));
}
else if (loginMode == "2")
{
    Console.WriteLine("VerifyToken()");
    var profile = await verifyClient.VerifyToken(session);
    Console.WriteLine(JsonSerializer.Serialize(profile));
}
else
{
    return;
}
