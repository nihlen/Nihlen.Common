using System.Net;
using System.Threading.Tasks;
using Nihlen.Common;
using Nihlen.Gamespy;

namespace Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var gs = new Gamespy3Service();
            //var result = await gs.QueryServerAsync(IPAddress.Parse("177.54.147.195"), 29900);
            var result = await gs.QueryServerAsync(IPAddress.Parse("95.172.92.116"), 29900);

            //var sw = Stopwatch.StartNew();
            //Console.WriteLine("Started");
            ////await TestPdfAsync();
            //await TestScreenshotAsync();
            //Console.WriteLine($"Finished in {sw.ElapsedMilliseconds} ms");
        }

        private static async Task TestPdfAsync()
        {
            var pdfGenerator = new HtmlPdfGenerator();
            await pdfGenerator.SaveUrlPdfAsync("https://www.google.com", @"C:\Projects\DotNet\Nihlen.Common\Test\test.pdf", false);
        }

        private static async Task TestScreenshotAsync()
        {
            var screenshotter = new WebsiteScreenshotter();
            await screenshotter.SaveAsync("https://www.google.com", @"C:\Projects\DotNet\Nihlen.Common\Test\hd.png");
        }
    }
}
