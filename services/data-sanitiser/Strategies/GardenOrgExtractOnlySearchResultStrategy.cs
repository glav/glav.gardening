using System;
using System.Collections.Generic;
using System.Text;

namespace Glav.DataSanitiser.Strategies
{
    public class GardenOrgExtractOnlySearchResultStrategy : IDataSanitiserStrategy
    {
        public SanitiseContentType ContentTypeSupported => SanitiseContentType.Html;

        public string SanitiseData(string content)
        {
            var startIndex = content.IndexOf("<caption>Search Results</caption>", 0, StringComparison.InvariantCultureIgnoreCase);
            if (startIndex < 0) return content;

            var endIndex = content.IndexOf("</tbody>", 0, StringComparison.InvariantCultureIgnoreCase);
            if (endIndex < 0) return content;
            endIndex += "</tbody>".Length;

            return content.Substring(startIndex,endIndex-startIndex);


        }
    }
}
