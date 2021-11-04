using System;
using System.Collections.Generic;
using System.Text;

namespace Glav.Gardening.Services.Agents.GardenOrg.Parsers
{
    public class GardenOrgSearchResultsParser
    {
        private const string _initialMarker = "<td data-th=\"\">";

        public List<GardenOrgSearchResultItem> ParseData(string content)
        {
            var searchResults = new List<GardenOrgSearchResultItem>();
            if (string.IsNullOrWhiteSpace(content))
            {
                return searchResults;
            }

            int pos=0,cnt=0;
            
            while (pos >= 0)
            {
                cnt++;
                pos = content.IndexOf(_initialMarker,pos+1);
                if (pos >= 0 && cnt % 2 == 0)
                {
                    var posStartQuotes = content.IndexOf('"',pos+_initialMarker.Length);
                    var posEndQuotes = content.IndexOf('"',posStartQuotes+1);
                    var hrefResult = content.Substring(posStartQuotes+1,posEndQuotes-posStartQuotes-1);
                    var posStartText = content.IndexOf('>',posEndQuotes);
                    var posEndText = content.IndexOf("</a>",posStartText);
                    var searchTextResult = content.Substring(posStartText+1,posEndText-posStartText-1);
                    searchResults.Add(new GardenOrgSearchResultItem{ Href = hrefResult, ResultText = searchTextResult.Replace("\n",string.Empty)});
                    pos = posEndText;
                }
            }
            return searchResults;
        }
    }

}
