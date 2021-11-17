using System.Net.Http;
using System.Threading.Tasks;

namespace Glav.Gardening.Communications
{
    public interface ICommunicationProxy
    {
        Task<string> GetContentAsync(string daprAppIdOrHost, string serviceMethod, string serviceVersion = "v1.0");
        Task<string> GetExternalContentAsync(string rawUrl);
        Task<string> PostContentAsync(string daprAppIdOrHost, string serviceMethod, HttpContent content = null, string serviceVersion = "v1.0");
        bool IsDaprEnvironment();
    }

}
