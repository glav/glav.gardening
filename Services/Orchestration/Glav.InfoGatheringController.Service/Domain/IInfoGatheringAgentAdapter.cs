using System.Threading.Tasks;

namespace Glav.InformationGathering.Domain
{
    /// <summary>
    /// Provides the contrract for any implementation adapter for each information gathering agent
    /// </summary>
    public interface IInfoGatheringAgentAdapter
    {
        Task StartAsync(string queryTerm);
        int Progress { get; }
        Task StopAsync();
    }    
}