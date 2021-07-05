using Glav.DataSanitiser;
using Glav.DataSanitiser.Sanitisers.Strategies;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Glav.DataSanitiser.Sanitisers
{
    public class GardenOrgSanitiser
    {
        private readonly ILogger _logger;
        public GardenOrgSanitiser(ILogger logger)
        {
            _logger = logger;
        }
        public List<GardenOrgSearchResultItem> SanitiseData(string data)
        {
            var sanitiseStrategies = new List<IDataSanitiserStrategy>();
            sanitiseStrategies.Add(new GardenOrgExtractOnlySearchResultStrategy());
            var engine = new DataSanitiserEngine( sanitiseStrategies, _logger);
            var cleanData = engine.SanitiseDataForAllContentTypes(data);

            // Parse the content to extract the search results
            var searchResultParser = new GardenOrgSearchResultsParser();
            var searchResults = searchResultParser.ParseData(cleanData);

            //Then clean up the search result text some more ensuring no HTML content is part of the text description
            engine.SanitiserStrategies.Clear();
            engine.SanitiserStrategies.Add(new RemoveHtmlStrategy());
            var finalResults = new List<GardenOrgSearchResultItem>();
            searchResults.ForEach(r => {
                finalResults.Add(r with {ResultText = engine.SanitiseDataForAllContentTypes(r.ResultText)});
            });
            return finalResults;
        }
    }

}