
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

class Program
{
    private static HttpClientManager httpClientManager;
    private static Downloader downloader;
    static async Task Main(string[] args)
    {
        httpClientManager = new HttpClientManager();
        downloader = new Downloader(httpClientManager.Client);

        while (true)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Download media files (mp3/mp4)");
            Console.WriteLine("2. Download images from a web page");
            Console.WriteLine("3. Exit");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await HandleDownload("media");
                    break;
                case "2":
                    await HandleDownload("images");
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private static async Task HandleDownload(string type)
    {
        List<string> urls = new List<string>();

        if (type == "media")
        {
            Console.WriteLine("Enter media URLs one by one. Type '1' to finish:");
            while (true)
            {
                string input = Console.ReadLine();
                if (input == "1") break;
                urls.Add(input);
            }
        }
        else if (type == "images")
        {
            Console.WriteLine("Enter the URL of the web page to download images from:");
            string url = Console.ReadLine();
            urls = await downloader.ExtractImageUrlsFromPage(url);
            if (urls.Count == 0)
            {
                Console.WriteLine("No images found.");
                return;
            }
        }

        string downloadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downloads");
        Directory.CreateDirectory(downloadsFolder);

        List<Task> downloadTasks = new List<Task>();
        for (int i = 0; i < urls.Count; i++)
        {
            int index = i;
            downloadTasks.Add(downloader.DownloadFileAsync(urls[index], downloadsFolder, type, index));
        }

        await Task.WhenAll(downloadTasks);
        Console.WriteLine($"All {type} files downloaded.");
    }
}