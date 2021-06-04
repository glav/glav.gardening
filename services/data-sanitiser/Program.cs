using System;
using System.Collections.Generic;
using System.IO;
using Glav.DataSanitiser.Diagnostics;
using Glav.DataSanitiser.Strategies;

namespace Glav.DataSanitiser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Data Sanitiser console bootstrapper");
            // if (args == null || args.Length < 1)
            // {
            //     Console.WriteLine("Usage: data-sanitiser {file-to-sanitise}");
            //     return;
            // }

            // if (!File.Exists(args[0]))
            // {
            //     Console.WriteLine($"Error: Unable to find file: [{args[0]}]");
            //     return;
            // }

            //var data = File.ReadAllText(args[0]);
            var data = File.ReadAllText("C:\\temp\\garden-search.html");;

            var sanitiseStrategies = new List<IDataSanitiserStrategy>();
            sanitiseStrategies.Add(new GardenOrgExtractOnlySearchResultStrategy());
            sanitiseStrategies.Add(new GardenOrgRemoveHtmlFromSearchResultStrategy());

            var engine = new DataSanitiserEngine( sanitiseStrategies, new ConsoleDiagnosticLogger());
            
            var cleanData = engine.SanitiseDataForAllContentTypes(data);
            Console.WriteLine();
            //Console.WriteLine(cleanData);
        }
    }
}
