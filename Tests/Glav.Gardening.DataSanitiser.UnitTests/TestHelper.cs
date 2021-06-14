using System;
using System.IO;
using System.Reflection;

namespace Glav.Gardening.DataSanitiser.UnitTests
{
    public class TestHelper
    {
        public string GetFileDataEmbeddedInAssembly(string filename)
        {
            var asm = this.GetType().GetTypeInfo().Assembly;
            using (var stream = asm.GetManifestResourceStream($"Glav.Gardening.UnitTests.Data.{filename}"))
            {
                using (var sr = new System.IO.StreamReader(stream))
                {
                    var testData = sr.ReadToEnd();
                    return testData;
                }
            }
        }

        public static string GetDataFile(string filename)
        {
            var fullPath = $"./Data/{filename}";
            if (!File.Exists(fullPath))
            {
                throw new Exception($"Invalid file or path: {fullPath}");
            }
            return File.ReadAllText(fullPath);
        }

    }
}
