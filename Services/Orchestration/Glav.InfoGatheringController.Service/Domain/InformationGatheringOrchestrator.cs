using Glav.Gardening.Communications;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Glav.InformationGathering.Domain
{
    public class InformationGatheringOrchestrator : IInformationGatheringOrchestrator
    {
        private readonly IDaprCommunicationProxy _daprProxy;
        private readonly ILogger<InformationGatheringOrchestrator> _logger;

        public InformationGatheringOrchestrator(ILogger<InformationGatheringOrchestrator> logger,IDaprCommunicationProxy daprProxy)
        {
            _logger = logger;
            _daprProxy = daprProxy;
        }
        public async Task InitiateAsync(string queryTerm)
        {
            //TODO: Initiated by a received msg on svc bus (typically)
            //TODO: Loop through agents
            //TODO: Get Url of each agent and invoke it.
            //TODO: Store results.

            _logger.LogInformation("Executing gardenorg agent");
            var result = await _daprProxy.GetContentAsync("gardenorgagent", $"GardenOrgAgent?queryTerm={queryTerm}");

            throw new NotImplementedException();
        }
    }
}