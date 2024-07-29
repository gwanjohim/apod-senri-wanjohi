# George Wanjohi - APOD

## Introduction

NASA Astronomy Picture of the Day (APOD) API is an initiative that provides a publicly accessible API to access imagery and associated metadata. This data can be repurposed for other applications. 

## Objective 

Create a console application using C# (.NET Core or .NET Framework) that consumes the
NASA Astronomy Picture of the Day (APOD) API, saves the picture to a file, and stores the
description in an organized manner by date. Additionally, the application should read settings
from a JSON file.

## Reading configuration from a json file

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


Once the configurations are loaded, we can pull the content from the APOD API as follows

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