using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Glav.DataSanitiser.Sanitisers.Strategies
{
    public class RemoveHtmlStrategy : IDataSanitiserStrategy
    {
        public SanitiseContentType ContentTypeSupported => SanitiseContentType.Html;

        public string SanitiseData(string content)
        {
            return Regex.Replace(content, "<.*?>", String.Empty);
        }
    }
}
