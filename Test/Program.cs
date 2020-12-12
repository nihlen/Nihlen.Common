using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Nihlen.Common;

namespace Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine("Started");
            //await TestPdfAsync();
            await TestScreenshotAsync();
            Console.WriteLine($"Finished in {sw.ElapsedMilliseconds} ms");
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
