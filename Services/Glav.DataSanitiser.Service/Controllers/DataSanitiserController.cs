using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Glav.DataSanitiser.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataSanitiserController : ControllerBase
    {
        private readonly ILogger<DataSanitiserController> _logger;

        public DataSanitiserController(ILogger<DataSanitiserController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/GardenOrgSearchResults")]  // when hosting via dapr, we want this to be at the root since we will be 
        //[HttpPost("GardenOrgSearchResults")]
        public GardenOrgSearchSanitisedData SanitiseGardenOrgSearchResults([FromBody]string dataToSanitise)
        {
            _logger.LogInformation("Parsing and extracting data for Garrden.Org");
            var gardenOrgParser = new GardenOrgSearchResultsParser();
            var searchResults = gardenOrgParser.ParseData(dataToSanitise);
            return new GardenOrgSearchSanitisedData { SearchResults = searchResults };


        }
    }
}
