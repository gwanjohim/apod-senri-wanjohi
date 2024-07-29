// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Text.Json;
using RestSharp;
using Senri_APOD_Wanjohi;
using Serilog;
using VideoLibrary;

public class APODIntegration
{
    public ApodConfiguration LoadConfiguration()
    {
        if (!File.Exists("AppSettings.json"))
        {
            throw new Exception("The Settings file cannot be found");
        }

        using StreamReader r = new StreamReader("AppSettings.json");
        string json = r.ReadToEnd();
        var config = JsonSerializer.Deserialize<ApodConfiguration>(json);
        return config!;
    }

    public APODAPIResult? PullImage(ApodConfiguration config)
    {
        var client = new RestClient(config.ApiBaseUrl);
        var request = new RestRequest("", Method.Get);
        request.AddParameter("api_key", config.ApiKey);
        request.AddParameter("date", config.Date);
        var response = client.Execute(request);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var code = response.StatusCode;
            Log.Warning($"Could not fetch data. The process failed with status code {code}");
            return null;
        }

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var apodApiResult = JsonSerializer.Deserialize<APODAPIResult>(response.Content);
            return apodApiResult;
        }

        Log.Error("Could not make a request to the APOD API, please check your internet connection and try again");
        return null;
    }


    public void SaveImageOfTheDay(APODAPIResult? imageData, string downloadDirectory,
        ApodConfiguration apodConfiguration)
    {
        var fileUrl = "";
        if (imageData != null)
            fileUrl = apodConfiguration.DownloadFileHdVersion ? imageData.hdurl : imageData.url;

        if (!string.IsNullOrWhiteSpace(fileUrl))
        {
            // var client = new RestClient(fileUrl);
            // var request = new RestRequest("#", Method.Get);
            // byte[]? response = client.DownloadData(request);


            var fileName = fileUrl.Substring(fileUrl.LastIndexOf('/') + 1);
            downloadDirectory += $"/{apodConfiguration.Date}";

            var imagePath = $"{downloadDirectory}/{fileName}";
            var descriptionPath = Path.Combine(downloadDirectory, "description.txt");

            if (!Directory.Exists(downloadDirectory))
            {
                Directory.CreateDirectory(downloadDirectory);
            }

            if (!File.Exists(descriptionPath))
            {
                using StreamWriter outputFile = new StreamWriter(descriptionPath);
                outputFile.WriteLine(imageData.explanation);
            }

            if (File.Exists(imagePath))
            {
                Log.Warning($"The file associated with date {apodConfiguration.Date} already exists");
                return;
            }

            if (imageData.media_type == "video")
            {
                // var VedioUrl = "https://www.youtube.com/embed/" + objYouTube.VideoID + ".mp4";
                var VedioUrl = imageData.url;
                var youTube = YouTube.Default;
                var video = youTube.GetVideo(VedioUrl);
                var videoSavingLocation = $"{downloadDirectory}/{imageData.title}.mp4";
                System.IO.File.WriteAllBytes(videoSavingLocation, video.GetBytes());
            }
            else
            {
                var stream = File.Create(imagePath);
                var webClient = new WebClient();
                var result = webClient.DownloadData(fileUrl);
                stream.Write(result);
                stream.Close();
            }
        }
        else
        {
            Log.Warning("The image data url was null and therefore no image could be downloaded");
        }
    }
}

public class Program
{
    public static int Main(string[] args)
    {
        using var log = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        var integration = new APODIntegration();

        var apodConfiguration = integration.LoadConfiguration();
        var imageResult = integration.PullImage(apodConfiguration);
        integration.SaveImageOfTheDay(imageResult, apodConfiguration.DownloadLocation, apodConfiguration);
        return 0;
    }
}