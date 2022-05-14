using Nihlen.Common.Abstractions;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Nihlen.Common;

public class HtmlPdfGenerator : IHtmlPdfGenerator
{
    private readonly PdfOptions _options;
    private readonly BrowserFetcher _browserFetcher;

    public HtmlPdfGenerator()
    {
        _options = new PdfOptions();
        _browserFetcher = new BrowserFetcher();
    }

    public async Task<Stream> GetUrlPdfAsync(string url, bool printMode = true)
    {
        var context = await GetUrlContextAsync(url, printMode);
        await using var browser = context.Browser;
        await using var page = context.Page;
        return await page.PdfStreamAsync(_options);
    }

    public async Task<Stream> GetHtmlPdfAsync(string htmlContent, bool printMode = true)
    {
        var context = await GetHtmlContextAsync(htmlContent, printMode);
        await using var browser = context.Browser;
        await using var page = context.Page;
        return await page.PdfStreamAsync(_options);
    }

    public async Task SaveUrlPdfAsync(string url, string outputFile, bool printMode = true)
    {
        var context = await GetUrlContextAsync(url, printMode);
        await using var browser = context.Browser;
        await using var page = context.Page;
        await page.PdfAsync(outputFile, _options);
    }

    public async Task SaveHtmlPdfAsync(string htmlContent, string outputFile, bool printMode = true)
    {
        var context = await GetHtmlContextAsync(htmlContent, printMode);
        await using var browser = context.Browser;
        await using var page = context.Page;
        await page.PdfAsync(outputFile, _options);
    }

    private async Task<Browser> GetBrowserAsync()
    {
        await _browserFetcher.DownloadAsync(BrowserFetcher.DefaultRevision);
        return await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
    }

    private async Task<(Browser Browser, Page Page)> GetUrlContextAsync(string url, bool printMode)
    {
        var browser = await GetBrowserAsync();
        var page = await browser.NewPageAsync();
        if (!printMode) await page.EmulateMediaTypeAsync(MediaType.Screen);
        await page.GoToAsync(url);
        return (browser, page);
    }

    private async Task<(Browser Browser, Page Page)> GetHtmlContextAsync(string htmlContent, bool printMode)
    {
        var browser = await GetBrowserAsync();
        var page = await browser.NewPageAsync();
        if (!printMode) await page.EmulateMediaTypeAsync(MediaType.Screen);
        await page.SetContentAsync(htmlContent);
        return (browser, page);
    }
}
