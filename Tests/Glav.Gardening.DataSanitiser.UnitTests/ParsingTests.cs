using Glav.DataSanitiser;
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
    }
}
