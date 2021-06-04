using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Glav.DataSanitiser.Strategies
{
    public class RemoveEncodedCharactersStrategy : IDataSanitiserStrategy
    {
        private static List<EncodedCharReplacement> EncodedCharacters = new List<EncodedCharReplacement>
        {
            new EncodedCharReplacement("&nbsp;"," "),
            new EncodedCharReplacement("&amp;","&"),
            new EncodedCharReplacement("&quot;"," ")
        };

        public SanitiseContentType ContentTypeSupported => SanitiseContentType.Html;

        public string SanitiseData(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return content;
            }
            var builder = new StringBuilder(content);
            EncodedCharacters.ForEach(e =>
            {
                builder.Replace(e.EncodedString, e.ReplacementString);
            });
            return builder.ToString();
        }

        private class EncodedCharReplacement
        {
            public EncodedCharReplacement(string encoded, string replacement)
            {
                EncodedString = encoded;
                ReplacementString = replacement;
            }
            public string EncodedString { get; set; }
            public string ReplacementString { get; set; }
        }
    }

}
