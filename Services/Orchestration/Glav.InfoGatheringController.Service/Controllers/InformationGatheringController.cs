using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Glav.Gardening.Communications;
using Glav.InformationGathering.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Glav.InformationGathering.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InformationGatheringController : ControllerBase
    {
        private readonly ILogger<InformationGatheringController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IInformationGatheringOrchestrator _orchestrator;

        public InformationGatheringController(ILogger<InformationGatheringController> logger, IConfiguration configuration, IInformationGatheringOrchestrator orchestrator)
        {
            _logger = logger;
            _configuration = configuration;
            _orchestrator = orchestrator;
        }

        [HttpPost("/start")]  // Used for dapr as dapr sidecar uses the app id in url making controller redundant
        //[HttpPost("start")]  // for standard /controller/method routing
        public async Task StartGatheringInformation(string queryTerm)
        {
            _logger.LogInformation("Starting information gathering process");

            var endpoint = _configuration["endpoints:datasanitiser"];

            // Ensure we use the 'DAPR_HTTP_PORT' environment variable here and replace with the port below
            //var daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
            //Console.WriteLine($"DAPR-PORT: {daprPort}");

            await _orchestrator.InitiateAsync(queryTerm);
            //var result = await _daprProxy.GetContentAsync("gardenorgagent", "GardenOrgAgent");
            //var daprEndpoint = $"localhost:{daprPort}/v1.0/invoke/sanitiserapp/method/GardenOrgSearchResults";
            // Kick off the gathering process to retrieve data from all known sources

            await Task.FromResult(0);
        }
    }
}
