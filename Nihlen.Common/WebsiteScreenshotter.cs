//using Nihlen.Common.Abstractions;
//using PuppeteerSharp;

//namespace Nihlen.Common;

//public class WebsiteScreenshotter : IWebsiteScreenshotter
//{
//    private readonly ScreenshotOptions _options;
//    private readonly BrowserFetcher _browserFetcher;

//    public WebsiteScreenshotter()
//    {
//        _options = new ScreenshotOptions { FullPage = true };
//        _browserFetcher = new BrowserFetcher();
//    }

//    public async Task<Stream> GetAsync(string url)
//    {
//        var context = await GetUrlContextAsync(url);
//        await using var browser = context.Browser;
//        await using var page = context.Page;
//        return await page.ScreenshotStreamAsync(_options);
//    }

//    public async Task SaveAsync(string url, string outputFile)
//    {
//        var context = await GetUrlContextAsync(url);
//        await using var browser = context.Browser;
//        await using var page = context.Page;
//        await page.ScreenshotAsync(outputFile, _options);
//    }

//    private async Task<Browser> GetBrowserAsync()
//    {
//        await _browserFetcher.DownloadAsync(BrowserFetcher.DefaultRevision);
//        return await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
//    }

//    private async Task<(Browser Browser, Page Page)> GetUrlContextAsync(string url)
//    {
//        var browser = await GetBrowserAsync();
//        var page = await browser.NewPageAsync();
//        await page.GoToAsync(url);
//        return (browser, page);
//    }
//}
