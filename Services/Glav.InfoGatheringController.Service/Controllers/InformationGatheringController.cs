using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public InformationGatheringController(ILogger<InformationGatheringController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("start")]
        public async Task StartGatheringInformation()
        {
            _logger.LogInformation("Starting information gathering process");

            var endpoint = _configuration["endpoints:datasanitiser"];

            // Kick off the gathering process to retrieve data from all known sources
            await Task.FromResult(0);
        }
    }
}
