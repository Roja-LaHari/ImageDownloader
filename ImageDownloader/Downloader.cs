//csharp
// Downloader.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

public class Downloader
{
    private readonly HttpClient client;

    public Downloader(HttpClient httpClient)
    {
        client = httpClient;
    }

    public async Task<List<string>> ExtractImageUrlsFromPage(string url)
    {
        List<string> imageUrls = new List<string>();
        try
        {
            string htmlContent = await client.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);
            var imageNodes = doc.DocumentNode.SelectNodes("//img[@src]");

            if (imageNodes != null)
            {
                foreach (var img in imageNodes)
                {
                    string src = img.GetAttributeValue("src", null);
                    if (!string.IsNullOrEmpty(src))
                    {
                        if (!Uri.IsWellFormedUriString(src, UriKind.Absolute))
                        {
                            Uri baseUri = new Uri(url);
                            Uri fullUri = new Uri(baseUri, src);
                            src = fullUri.ToString();
                        }
                        imageUrls.Add(src);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while extracting image URLs: {ex.Message}");
        }
        return imageUrls;
    }

    public async Task DownloadFileAsync(string url, string downloadFolder, string type, int index)
    {
        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string mediaType = response.Content.Headers.ContentType?.MediaType;
            Console.WriteLine($"Received Content-Type: {mediaType}");
            byte[] mediaBytes = await response.Content.ReadAsByteArrayAsync();

            string extension = GetFileExtension(mediaType);
            if (extension != null)
            {
                string sanitizedFileName = SanitizeFileName($"downloaded_{type}_{index}");
                string filePath = Path.Combine(downloadFolder, sanitizedFileName + extension);
                await File.WriteAllBytesAsync(filePath, mediaBytes);
                Console.WriteLine($"File saved :{sanitizedFileName}");
            }
            else
            {
                Console.WriteLine($"Unsupported format: {mediaType}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private string GetFileExtension(string mediaType) => mediaType switch
    {
        "audio/mpeg" => ".mp3",
        "video/mp4" => ".mp4",
        "video/mpeg" => ".mpeg",
        "video/webm" => ".webm",
        "image/jpeg" => ".jpg",
        "image/png" => ".png",
        "image/gif" => ".gif",
        "image/bmp" => ".bmp",
        "image/webp" => ".webp",
        "image/tiff" => ".tiff",
        "image/svg+xml" => ".svg",
        "image/x-icon" => ".ico",
        _ => null
    };

    private string SanitizeFileName(string fileName)
    {
        string invalidChars = new string(Path.GetInvalidFileNameChars());
        string invalidRegex = $"[{Regex.Escape(invalidChars)}]";
        return Regex.Replace(fileName, invalidRegex, "_");
    }
}