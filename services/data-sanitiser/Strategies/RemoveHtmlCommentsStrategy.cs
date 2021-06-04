using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Glav.DataSanitiser.Strategies
{
    public class RemoveHtmlCommentsStrategy : IDataSanitiserStrategy
    {
        const string startComment = "<!--";
        const string endComment = "-->";
        public SanitiseContentType ContentTypeSupported => SanitiseContentType.Html;

        public string SanitiseData(string content)
        {
            var startPos = content.IndexOf(startComment);
            var buffer = new StringBuilder(content);
            while(startPos >= 0)
            {
                var endPos = content.IndexOf(endComment, startPos);
                if (endPos > startPos)
                {
                    buffer.Remove(startPos, endPos - startPos + 3);
                    content = buffer.ToString();
                    startPos = content.IndexOf(startComment);
                } else break;
            }
            return content;
        }
    }
}
