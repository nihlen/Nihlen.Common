namespace Nihlen.Common.Abstractions
{
    public interface IWebsiteScreenshotter
    {
        Task<Stream> GetAsync(string url);
        Task SaveAsync(string url, string outputFile);
    }
}
