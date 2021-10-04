using Glav.Gardening.Services.Agents.GardenOrg.Parsers;
using System;
using Xunit;

namespace Glav.Gardening.DataSanitiser.UnitTests
{
    public class ParsingTests
    {
        [Fact]
        public void ShouldExtractSearchResultsFromGardenOrgWebPageSearch()
        {
            var data = TestHelper.GetDataFile("garden-search.html");
            var gardenOrgParser = new GardenOrgSearchResultsParser();
            var searchResults = gardenOrgParser.ParseData(data);
            Assert.NotNull(searchResults);
            Assert.Equal(20, searchResults.Count);

        }

        [Fact]
        public void ShouldExtractItemResultDetailsFromGardenOrgWebPageSearchItem()
        {
            var data = TestHelper.GetDataFile("gardenorg-search-result-details.html");
            var gardenOrgParser = new GardenOrgSearchResultDetailsParser();
            var itemResults = gardenOrgParser.ParseData(data);
            Assert.NotNull(itemResults);
            Assert.Equal("Herb/Forb", itemResults.Habit);
            Assert.Equal("Full Sun to Partial Shade", itemResults.SunRequirements);
            Assert.Equal("Unusual foliage color, Fragrant", itemResults.Leaves);
        }
    }
}
