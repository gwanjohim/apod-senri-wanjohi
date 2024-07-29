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
        apodConfiguration.Date =  "2024-07-21";
        var imageResult = integration.PullImage(apodConfiguration);
        integration.SaveImageOfTheDay(imageResult, apodConfiguration.DownloadLocation, apodConfiguration);
        Assert.NotNull(imageResult);
    }

    [Fact]
    public void WhenFileIsVide_SavesSuccessfully()
    {
        var integration = new APODIntegration();
        var apodConfiguration = integration.LoadConfiguration();
        apodConfiguration.Date = "2024-07-28"; //on this date the file is a video
        var imageResult = integration.PullImage(apodConfiguration);
        integration.SaveImageOfTheDay(imageResult, apodConfiguration.DownloadLocation, apodConfiguration);
        Assert.NotNull(imageResult);
    }
    
    [Fact]
    public void CanPullItemsOverAWideRangeOfDates()
    {
        var startDate = DateTime.Parse("2024-1-1");

        while (startDate < DateTime.Now)
        {
            var integration = new APODIntegration();
            var apodConfiguration = integration.LoadConfiguration();
            apodConfiguration.Date = startDate.ToString("yyyy-MM-dd"); //on this date the file is a video
            var imageResult = integration.PullImage(apodConfiguration);
            integration.SaveImageOfTheDay(imageResult, apodConfiguration.DownloadLocation, apodConfiguration);
            Assert.NotNull(imageResult);

            startDate = startDate.AddDays(1);
            
        }
       
         
        
        
    }
}