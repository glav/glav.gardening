using System;
using System.Threading.Tasks;

namespace Glav.InformationGathering.Domain
{
    public class InformationGatheringOrchestrator : IInformationGatheringOrchestrator
    {
        public async Task InitiateAsync(string queryTerm)
        {
            //TODO: Initiated by a received msg on svc bus (typically)
            //TODO: Loop through agents
            //TODO: Get Url of each agent and invoke it.
            //TODO: Store results.

            throw new NotImplementedException();
        }
    }
}