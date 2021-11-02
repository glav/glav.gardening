using System.Threading.Tasks;

namespace Glav.Gardening.Communications
{
    public interface ICommunicationProtocol
    {
        Task<string> GetContentAsync(string queryUrl);
    }

}
