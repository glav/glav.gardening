using System.Threading.Tasks;

namespace Glav.InformationGathering.Domain
{
    public interface IInfoGatheringAgent
    {
        Task StartAsync(string queryTerm);
        int Progress { get; }
        Task StopAsync();
    }    
}