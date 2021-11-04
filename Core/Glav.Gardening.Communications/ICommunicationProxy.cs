using System.Threading.Tasks;

namespace Glav.Gardening.Communications
{
    public interface ICommunicationProxy
    {
        Task<string> GetContentAsync(string queryUrl);
    }

}
