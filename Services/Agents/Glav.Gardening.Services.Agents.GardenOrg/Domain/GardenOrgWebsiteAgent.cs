using Glav.Gardening.Services.Agents.GardenOrg.Parsers;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Glav.InformationGathering.Domain.GardenOrg.Domain
{
    public class GardenOrgWebsiteAgent
    {
        private readonly ILogger<GardenOrgWebsiteAgent> _logger;
        private int _progress = 0;
        const string queryUrl = "https://garden.org/plants/search/text/?q={0}";

        public GardenOrgWebsiteAgent(ILogger<GardenOrgWebsiteAgent> logger)
        {
            _logger = logger;
        }

        public int Progress => _progress;

        public async Task StartAsync(string queryTerm)
        {
            var client = new HttpClient();

            //Note: use Polly or some retry mechanism here - try..catch for now
            try
            {
                var result = await client.GetStringAsync(string.Format(queryUrl, queryTerm));
                _progress = 20;

                var searchResultParser = new GardenOrgSearchResultsParser();
                var searchResults = searchResultParser.ParseData(result);
                if (searchResults?.Count == 0 || searchResults.All(r => string.IsNullOrEmpty(r.Href)))
                {
                    _logger.LogWarning("No results from GardenOrg query or invalid results return using term [{0}]", queryTerm);
                    _progress = 100;
                    return;
                }
                _progress = 40;

                // 1. Pass search results into next component to make additional queries against GardenOrg to create a GardenOrgPlantItem for each result
                _progress = 80;

                // 2. Store results into storage
                _progress = 100;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem querying GardenOrg site");
            }
        }

        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}