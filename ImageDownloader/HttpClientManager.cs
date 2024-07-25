//csharp
// HttpClientManager.cs
using System.Net.Http;
using System.Net.Http.Headers;

public class HttpClientManager
{
    private readonly HttpClient client;

    public HttpClientManager()
    {
        client = new HttpClient();
        ConfigureHttpClient();
    }

    public HttpClient Client => client;

    private void ConfigureHttpClient()
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd(GetUserAgent());
    }

    private string GetUserAgent() => "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";
}