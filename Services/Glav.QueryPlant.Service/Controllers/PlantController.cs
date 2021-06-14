using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Glav.QueryPlant.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlantController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Mint", "Chilli", "Rosemary", "Spring onions", "Tomatoes", "Spinach", "Sage", "Coriander"
        };

        private readonly ILogger<PlantController> _logger;

        public PlantController(ILogger<PlantController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<PlantSummary> Get([FromQuery]string query)
        {
            _logger.LogInformation("Querying for plant information: [{0}]",query);

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new PlantSummary
            {
                Date = DateTime.Now.AddDays(index),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
