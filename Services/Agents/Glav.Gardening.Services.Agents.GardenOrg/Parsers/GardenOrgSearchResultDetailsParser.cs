﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Glav.Gardening.Services.Agents.GardenOrg.Parsers
{
    public class GardenOrgSearchResultDetailsParser
    {
        private const string _initialMarker = "<caption>General Plant Information";

        public GardenOrgSearchResultDetail ParseData(string content)
        {
            var item = new GardenOrgSearchResultDetail();
            if (string.IsNullOrWhiteSpace(content))
            {
                return item;
            }

            int pos = 0;

            pos = content.IndexOf(_initialMarker, pos + 1);
            pos = content.IndexOf("<tbody>", pos);

            pos = content.IndexOf("Plant Habit", pos);
            var plantHabitContent = GetElementDataForTd(content, pos);
            pos = content.IndexOf("Sun Requirements", pos);
            var sunRqmntsContent = GetElementDataForTd(content, pos);
            pos = content.IndexOf("Leaves", pos);
            var leaves = GetElementDataForTd(content, pos);

            return new GardenOrgSearchResultDetail { 
                PlantHabit = plantHabitContent, 
                SunRequirements = sunRqmntsContent,
                Leaves = leaves
             };
        }
        private string[] GetElementDataForTd(string content, int startPos)
        {
            int pos = content.IndexOf("<td", startPos);
            int endPos = content.IndexOf("</td>",pos);
            var tdContent = content.Substring(pos, endPos-pos+1);
            // Check for a span in the TD
            int spos = tdContent.IndexOf("<span");
            if (spos >= 0)
            {
                spos = tdContent.IndexOf(">", spos + 7);
                int sEndPos = tdContent.IndexOf("</span>");
                return StripHtmlAndRemoveCrLf(tdContent.Substring(spos + 1, sEndPos - spos - 1));
            }
            // no span, lets just extract
            pos = content.IndexOf(">", pos);
            return StripHtmlAndRemoveCrLf(content.Substring(pos + 1, endPos - pos - 1));
        }

        private string[] StripHtmlAndRemoveCrLf(string content)
        {
            var cleanedContent = Regex.Replace(content, "<.*?>", String.Empty).Split("\r\n").ToList();
            if (cleanedContent == null || cleanedContent.Count == 0)
            {
                return new string[] { };
            }

            cleanedContent.ForEach(item =>
            {
                item = item
                    .Replace("\n", string.Empty)
                    .Replace("\r", string.Empty);

            });
            if (string.IsNullOrWhiteSpace(cleanedContent[cleanedContent.Count-1]))
            {
                cleanedContent.RemoveAt(cleanedContent.Count - 1);
            }
            return cleanedContent.ToArray();

        }
    }

}
