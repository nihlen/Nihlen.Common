using System.IO;
using System.Threading.Tasks;

namespace Nihlen.Common.Abstractions
{
    public interface IHtmlPdfGenerator
    {
        Task<Stream> GetUrlPdfAsync(string url, bool printMode = true);
        Task<Stream> GetHtmlPdfAsync(string htmlContent, bool printMode = true);
        Task SaveUrlPdfAsync(string url, string outputFile, bool printMode = true);
        Task SaveHtmlPdfAsync(string htmlContent, string outputFile, bool printMode = true);
    }
}
