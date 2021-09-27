using Glav.Gardening.Services.Agents.GardenOrg.Parsers;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Glav.InformationGathering.Domain.GardenOrg.Domain
{
    public class GardenOrgWebsiteAgent
    {
        private readonly ILogger<GardenOrgWebsiteAgent> _logger;
        private int _progress = 0;
        const string queryUrl = "https://garden.org/plants/search/text/?q={0}";

        public GardenOrgWebsiteAgent(ILogger<GardenOrgWebsiteAgent> logger)
        {
            _logger = logger;
        }

        public int Progress => _progress;

        public async Task StartAsync(string queryTerm)
        {
            var content = await GetContentAsync(queryTerm);

            //Note: use Polly or some retry mechanism here - try..catch for now
            try
            {
                _progress = 20;

                if (string.IsNullOrWhiteSpace(content))
                {
                    _progress = 100;
                    return;
                }
                var searchResultParser = new GardenOrgSearchResultsParser();
                var searchResults = searchResultParser.ParseData(content);
                if (searchResults?.Count == 0 || searchResults.All(r => string.IsNullOrEmpty(r.Href)))
                {
                    _logger.LogWarning("No results from GardenOrg query or invalid results return using term [{0}]", queryTerm);
                    _progress = 100;
                    return;
                }
                _progress = 40;

                // 1. Pass search results into next component to make additional queries against GardenOrg to create a GardenOrgPlantItem for each result
                _progress = 80;

                // 2. Store results into storage
                _progress = 100;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem querying GardenOrg site");
            }
        }

        private async Task<string> GetContentAsync(string queryTerm)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("br"));
            client.DefaultRequestHeaders.Connection.Add("keep-alive");
            client.DefaultRequestHeaders.Add("User-Agent", "glav/agent - gardenorg");  // Must add this or gardenOrg returns no results.
            var result = await client.GetAsync(string.Format(queryUrl, queryTerm));
            var content = await GetZipBody(result);
            return content;

        }

        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> GetZipBody(HttpResponseMessage rspMsg)
        {
            string body = null;
            Stream zipStr = null;
            var compressionType = GetCompressionType(rspMsg);
            if (compressionType == CompressionType.Gzip)
            {
                zipStr = new System.IO.Compression.GZipStream(await rspMsg.Content.ReadAsStreamAsync(), System.IO.Compression.CompressionMode.Decompress);
                _logger.LogInformation("Request headers contain GZIP - setting GZIP decompression");
            }
            else if (compressionType == CompressionType.Deflate)
            {
                zipStr = new System.IO.Compression.GZipStream(await rspMsg.Content.ReadAsStreamAsync(), System.IO.Compression.CompressionMode.Decompress);
                _logger.LogInformation("Request headers contain DEFLATE - setting DEFLATE decompression");
            }

            if (zipStr != null)
            {
                _logger.LogInformation("Request headers indicate compressed payload - attempting to decompress payload");
                try
                {
                    using (zipStr)
                    using (StreamReader sr = new StreamReader(zipStr))
                    {
                        body = sr.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to decompress GZIP payload, defaulting to string content");
                }
            }

            if (body == null)
            {
                _logger.LogInformation("Reading request payload as string");
                body = await rspMsg.Content.ReadAsStringAsync();
            }


            return body;
        }
        public static CompressionType GetCompressionType(HttpResponseMessage rspMsg)
        {
            var headers = rspMsg.Content.Headers
                .Where(h => h.Key.ToLowerInvariant() == "content-type" || h.Key.ToLowerInvariant() == "content-encoding")
                .SelectMany(i => i.Value);

            if (headers.Any(h => h.ToLowerInvariant().Contains("gzip")))
            {
                return CompressionType.Gzip;
            }
            if (headers.Any(h => h.ToLowerInvariant().Contains("deflate")))
            {
                return CompressionType.Deflate;
            }
            return CompressionType.None;
        }

        public enum CompressionType
        {
            None,
            Gzip,
            Deflate
        }

    }
}