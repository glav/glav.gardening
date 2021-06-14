using System;
using System.Collections.Generic;
using System.IO;
using Glav.DataSanitiser.Diagnostics;

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

            var gardenOrgParser = new GardenOrgParseSearchResults();
            var searchResults = gardenOrgParser.ParseData(data);
            searchResults.ForEach(r => {
                Console.WriteLine($"> Href: [{r.Href}]: {r.ResultText}");
            });
            
            Console.WriteLine();
            //Console.WriteLine(cleanData);
        }
    }
}
