// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Text.Json;
using RestSharp;
using Senri_APOD_Wanjohi;
using Serilog;

using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

ApodConfiguration LoadConfiguration()
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

APODAPIResult? PullImage(ApodConfiguration config)
{
    var client = new RestClient(config.ApiBaseUrl);
    var request = new RestRequest("", Method.Get);
    request.AddParameter("api_key", config.ApiKey);
    request.AddParameter("date", config.Date);
    var response = client.Execute(request);
    if (response.ErrorMessage != null)
    {
        log.Error(response.ErrorMessage);
        return null;
  
    }
    if(response.Content != null)
    {
        var apodApiResult = JsonSerializer.Deserialize<APODAPIResult>(response.Content);
        return apodApiResult;
    }
    
    log.Error("Could not make a request to the APOD API, please check your internet connection and try again");
    return null;

}


void SaveImageOfTheDay(APODAPIResult? imageData, string downloadDirectory, string fetchDate)
{
    if (imageData is { url: not null })
    {
        var client = new RestClient(imageData.url);
        var request = new RestRequest("", Method.Get);
        byte[]? response = client.DownloadData(request);


        var fileName = imageData.url.Substring(imageData.url.LastIndexOf('/') + 1);
        downloadDirectory += $"/{fetchDate}";

        var imagePath = $"{downloadDirectory}/{fileName}";
        var descriptionPath = Path.Combine(downloadDirectory, "description.txt");

        if (!Directory.Exists(downloadDirectory))
        {
            Directory.CreateDirectory(downloadDirectory);
        }

        if (File.Exists(imagePath)){
            log.Warning($"The file associated with date {fetchDate} already exists");
            return;
        }
        var stream = File.Create(imagePath);
        stream.Write(response);
        stream.Close();

        if (!File.Exists(descriptionPath))
        {
            using StreamWriter outputFile = new StreamWriter(descriptionPath);
            outputFile.WriteLine(imageData.explanation);
        }
    }
    else
    {
        Log.Warning("The image data url was null and therefore no image could be downloaded");
    }

}

var apodConfiguration = LoadConfiguration();
var imageResult = PullImage(apodConfiguration);
SaveImageOfTheDay(imageResult, apodConfiguration.DownloadLocation, apodConfiguration.Date);