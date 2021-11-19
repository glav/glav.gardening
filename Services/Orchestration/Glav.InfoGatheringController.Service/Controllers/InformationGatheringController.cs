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
        private readonly IInformationGatheringOrchestrator _orchestrator;

        public InformationGatheringController(ILogger<InformationGatheringController> logger, IInformationGatheringOrchestrator orchestrator)
        {
            _logger = logger;
            _orchestrator = orchestrator;
        }

        [HttpPost("/start")]  // Used for dapr as dapr sidecar uses the app id in url making controller redundant
        //[HttpPost("start")]  // for standard /controller/method routing
        public async Task StartGatheringInformation(string queryTerm)
        {
            _logger.LogInformation("Starting information gathering process");

            await _orchestrator.InitiateAsync(queryTerm);

            await Task.FromResult(0);
        }
    }
}
