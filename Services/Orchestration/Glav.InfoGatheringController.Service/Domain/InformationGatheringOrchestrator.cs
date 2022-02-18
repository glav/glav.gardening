using Glav.Gardening.Communications;
using Glav.InformationGathering.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Glav.InformationGathering.Domain
{
    public class InformationGatheringOrchestrator : IInformationGatheringOrchestrator
    {
        private readonly ICommunicationProxy _commsProxy;
        private readonly ILogger<InformationGatheringOrchestrator> _logger;
        private readonly LocalFallbackAddress _fallbackConfig;

        public InformationGatheringOrchestrator(ILogger<InformationGatheringOrchestrator> logger, LocalFallbackAddress fallbackConfig, ICommunicationProxy commsProxy)
        {
            _logger = logger;
            _commsProxy = commsProxy;
            _fallbackConfig = fallbackConfig;
        }
        public async Task InitiateAsync(string queryTerm)
        {
            //TODO: Initiated by a received msg on svc bus (typically)
            //TODO: Loop through agents
            
            //TODO: Get Url of each agent and invokesend another message to invoke each one OR just get agents to listen via the topic message????

            //TODO: If doing sync via this, then store results??



            _logger.LogInformation("Executing gardenorg agent");
            _logger.LogInformation("Initiating GardenOrg agent");
            var gardenAgentAppId = "gardenorgagent";
            var serviceMethod = $"GardenOrgAgent?queryTerm={queryTerm}";
            var result = await _commsProxy.PostContentAsync(gardenAgentAppId, serviceMethod);

            throw new NotImplementedException("Yeah - orchestrator not finished - sorry");
        }
    }
}