using System.IO;
using System.Threading.Tasks;

namespace Nihlen.Common.Abstractions
{
    public interface IWebsiteScreenshotter
    {
        Task<Stream> GetAsync(string url);
        Task SaveAsync(string url, string outputFile);
    }
}
