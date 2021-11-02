using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Glav.Gardening.Communications
{
    public class HttpProtocol : ICommunicationProtocol
    {
        private readonly ILogger<HttpProtocol> _logger;

        public HttpProtocol(ILogger<HttpProtocol> logger)
        {
            _logger = logger;
        }
        public async Task<string> GetContentAsync(string queryUrl)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("br"));
            client.DefaultRequestHeaders.Connection.Add("keep-alive");
            client.DefaultRequestHeaders.Add("User-Agent", "query/agent - gardenorg");  // Must add this or gardenOrg returns no results.
            var result = await client.GetAsync(queryUrl);
            var content = await GetZipBody(result);
            return content;

        }

        public async Task<string> GetZipBody(HttpResponseMessage rspMsg)
        {
            string body = null;
            Stream zipStr = null;
            var compressionType = rspMsg.GetCompressionType();
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
    }

}
