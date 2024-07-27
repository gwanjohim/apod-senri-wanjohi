using System.ComponentModel.DataAnnotations;

namespace Senri_APOD_Wanjohi;

public class ApodConfiguration
{
    public string ApiBaseUrl { get; set; } = "";
    public string ApiKey { get; set; } = "0eU45WlKbhS4AgbFDrjcIUuv5ZuxKuD6zxjo0KpV";
    public string Date { get; set; } = "";
    public string DownloadLocation { get; set; } = "";
}
public class APODAPIResult
{
    public DateOnly date { get; set; }
    public string explanation { get; set; }
    public string hdurl { get; set; }
    public string media_type { get; set; }
    public string service_version { get; set; }
    public string title { get; set; }
    public string url { get; set; }
}