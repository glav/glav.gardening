using System.Threading.Tasks;

namespace Glav.InformationGathering.Domain
{
    public interface IInfoGatheringAgent
    {
        Task StartAsync(string queryTerm);
        Task<int> Progress();
        Task StopAsync();
    }    
}