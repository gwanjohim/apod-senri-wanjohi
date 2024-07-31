# George Wanjohi - APOD

## Introduction

NASA Astronomy Picture of the Day (APOD) API is an initiative that provides a publicly accessible API to access imagery and associated metadata. This data can be repurposed for other applications. 

## Objective 

Create a console application using C# (.NET Core or .NET Framework) that consumes the
NASA Astronomy Picture of the Day (APOD) API, saves the picture to a file, and stores the
description in an organized manner by date. Additionally, the application should read settings
from a JSON file.

## To run the project

navigate to Senri_APOD_Wanjohi folder, restore packages as follows

```bash 
 > dotnet restore
```

update the configurations in AppSettings.json

```json
{
  "ApiBaseUrl": "https://api.nasa.gov/planetary/apod",
  "ApiKey": API_KEY_GOES_HERE",
  "Date": "2024-12-12",
  "DownloadLocation": "/home/elite/Documents/APOD_Images",
}
```

Note: The download Location should be an **absolute path.** 

The date format is as provided here, **yyyy-mm-dd**


To run the application, 


```sh
> dotnet run
```

# How the Program works

## 1. Reading configuration from a json file

The configuration class has the following properties

```c#
public class ApodConfiguration
{
    public string ApiBaseUrl { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string Date { get; set; } = "";
    public string DownloadLocation { get; set; } = "";
    public bool DownloadFileHdVersion { get; set; } = false;
}
```

To load the configuration the following method is invoked

```c#
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
```


## 2. Once the configurations are loaded, we can pull the content from the APOD API as follows

```C#
     public APODAPIResult? PullDocument(ApodConfiguration config)
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

```

## 3. After successful download, the next code snippet illustrates how saving the fetched data works

```c#
   public void SaveImageOfTheDay(APODAPIResult? imageData, string downloadDirectory,
        ApodConfiguration apodConfiguration)
    {
        var fileUrl = imageData.url;
       
        if (!string.IsNullOrWhiteSpace(fileUrl))
        {
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
                var videoUrl = imageData.url;
                var youTube = YouTube.Default;
                var video = youTube.GetVideo(videoUrl);
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
```

Libraries used in this project

```c#
      <PackageReference Include="RestSharp" Version="111.4.0" />
      <PackageReference Include="Serilog" Version="4.0.1" />
      <PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
      <PackageReference Include="VideoLibrary" Version="3.2.4" />
```

1. Restsharp is used as a wrapper on top of http client library

2. Serilog is used to Log messages on the console

3. VideoLibrary is a package to download videos from youtube


## Closing remarks

    You will also find a test project running on XUnit. There are unit tests to test for some critical scenraios


Prepared and presented by George Wanjohi

1. Email: gwanjohim@gmail.com
2. Cell: 0707007337
