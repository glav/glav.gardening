using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace info_gathering_controller.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InformationGatheringController : ControllerBase
    {
        private readonly ILogger<InformationGatheringController> _logger;

        public InformationGatheringController(ILogger<InformationGatheringController> logger)
        {
            _logger = logger;
        }

        [HttpPost("start")]
        public async Task StartGatheringInformation()
        {
            _logger.LogInformation("Starting information gathering process");

            // Kick off the gathering process to retrieve data from all known sources
            await Task.FromResult(0);
        }
    }
}
