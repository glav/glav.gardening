using Glav.Gardening.Communications;
using Glav.Gardening.Services.Agents.GardenOrg.Parsers;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Glav.InformationGathering.Domain.GardenOrg.Domain
{
    public class GardenOrgWebsiteAgent
    {
        private readonly ILogger<GardenOrgWebsiteAgent> _logger;
        private int _progress = 0;
        const string _host = "garden.org";
        private readonly ICommunicationProxy _commsProxy;
        public GardenOrgWebsiteAgent(ILogger<GardenOrgWebsiteAgent> logger, ICommunicationProxy commsProxy)
        {
            _logger = logger;
            _commsProxy = commsProxy;
        }

        public int Progress => _progress;

        public async Task StartAsync(string queryTerm)
        {
            // Get some search results.
            var content = await _commsProxy.GetExternalContentAsync($"https://{_host}/plants/search/text/?q={queryTerm}");

            //Note: use Polly or some retry mechanism here - try..catch for now
            try
            {
                _progress = 20;

                if (string.IsNullOrWhiteSpace(content))
                {
                    _progress = 100;
                    return;
                }
                var searchResultParser = new GardenOrgSearchResultsParser();
                var searchResults = searchResultParser.ParseData(content);
                if (searchResults?.Count == 0 || searchResults.All(r => string.IsNullOrEmpty(r.Href)))
                {
                    _logger.LogWarning("No results from GardenOrg query or invalid results return using term [{0}]", queryTerm);
                    _progress = 100;
                    return;
                }
                _progress = 40;

                // 1. Pass search results into next component to make additional queries against GardenOrg to create a GardenOrgPlantItem for each result
                foreach (var result in searchResults)
                {
                    var detailContent = await _commsProxy.GetExternalContentAsync($"https://{_host}{result.Href}");
                    var parsedDetail = new GardenOrgSearchResultDetailsParser().ParseData(detailContent);
                    // TODO: Add to list of results to return
                }
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