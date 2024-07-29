using System.Runtime.InteropServices.JavaScript;

namespace Senri_APOD_Wanjohi_Test;

public class UnitTests
{
    [Fact]
    public void WhenDateIsGreaterThanTodayDownloadFails()
    {
        var integration = new APODIntegration();
        var apodConfiguration = integration.LoadConfiguration();
        apodConfiguration.Date = DateTime.Now.Date.Add(TimeSpan.FromDays(1)).ToString();
        var imageResult = integration.PullImage(apodConfiguration);
        Assert.Null(imageResult);
    }

    [Fact]
    public void WhenDateIsInRangeDownloadWorks()
    {
        var integration = new APODIntegration();
        var apodConfiguration = integration.LoadConfiguration();
        apodConfiguration.Date = DateTime.Now.Date.Subtract(TimeSpan.FromDays(1)).Date.ToString("yyyy-MM-dd");
        var imageResult = integration.PullImage(apodConfiguration);
        Assert.NotNull(imageResult);
    }

    [Fact]
    public void WhenFileIsVide_SavesSuccessfully()
    {
        var integration = new APODIntegration();
        var apodConfiguration = integration.LoadConfiguration();
        apodConfiguration.Date = "2024-07-28";
        var imageResult = integration.PullImage(apodConfiguration);
        imageResult.url = "https://youtu.be/wDchsz8nmbo";

        integration.SaveImageOfTheDay(imageResult, apodConfiguration.DownloadLocation, apodConfiguration);
        Assert.NotNull(imageResult);
    }
}