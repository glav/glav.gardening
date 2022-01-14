using Glav.InformationGathering.Domain.GardenOrg.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Glav.Gardening.Services.Agents.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgentController : ControllerBase
    {
        private readonly ILogger<AgentController> _logger;
        private readonly GardenOrgWebsiteAgent _agent;

        public AgentController(ILogger<AgentController> logger, GardenOrgWebsiteAgent agent)
        {
            _logger = logger;
            _agent = agent;
        }

        [HttpPost("/GardenOrgAgent")]
        public async Task InitiateAsync([FromQuery] string queryTerm)
        {
            _logger.LogInformation("Initiating GardenOrgWebsiteAgent with queryTerm=[{0}]",queryTerm);
            await _agent.StartAsync(queryTerm);
        }
    }
}
