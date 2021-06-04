using System;
using System.Collections.Generic;
using System.Text;

namespace Glav.DataSanitiser.Strategies
{
    public class GardenOrgRemoveHtmlFromSearchResultStrategy : IDataSanitiserStrategy
    {
        private const string _initialMarker = "<td data-th=\"\">";
        public SanitiseContentType ContentTypeSupported => SanitiseContentType.Html;
        private List<GardenOrgExtractOnlySearchResult> _searchResults = new List<GardenOrgExtractOnlySearchResult>(); 

        public string SanitiseData(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return content;
            }
            var builder = new StringBuilder();

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
                    _searchResults.Add(new GardenOrgExtractOnlySearchResult{ Href = hrefResult, ResultText = searchTextResult});
                    Console.WriteLine($"> Href: [{hrefResult}]: {searchTextResult}");
                    pos = posEndText;

                }
            }


            return "";


        }
    }

    public class GardenOrgExtractOnlySearchResult
    {
        public string Href {get; set;}
        public string ResultText {get; set;}
    }
}
