using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Glav.Gardening.Communications
{
    public class HttpCommunicationProxy : ICommunicationProxy
    {
        private readonly ILogger<HttpCommunicationProxy> _logger;
        private const string USER_AGENT = "query/agent";

        public HttpCommunicationProxy(ILogger<HttpCommunicationProxy> logger)
        {
            _logger = logger;
        }

        public async Task<string> PostContentAsync(string daprAppIdOrHost, string serviceMethod, HttpContent content = null, string serviceVersion = "v1.0")
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
                client.DefaultRequestHeaders.Connection.Add("keep-alive");
                client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
                var result = await client.PostAsync(FormUrl(daprAppIdOrHost, serviceMethod, serviceVersion), content);
                return await result.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool IsDaprEnvironment()
        {
            return !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DAPR_HTTP_PORT"));
        }
        private string FormUrl(string daprAppId, string serviceMethod, string serviceVersion = "v1.0")
        {
            var appSvc = AppServices.ById(daprAppId);

            if (IsDaprEnvironment())
            {
                var port = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
                if (appSvc.appId == ServiceAppId.State || appSvc.appId == ServiceAppId.PubSub)
                {
                    return $"http://localhost:{port}/{serviceVersion}/{appSvc.appId}/{serviceMethod}";
                }
                var daprEndpoint = $"http://localhost:{port}/{serviceVersion}/invoke/{appSvc.appId}/method/{serviceMethod}";
                return daprEndpoint;
            }

            return $"https://{appSvc.localFallbackAddress}/{serviceMethod}"; // typically for local dev where daprIdOrHost might be http://localhost:5001 for example

        }

        public async Task<string> GetContentAsync(string daprAppIdOrHost, string serviceMethod, string serviceVersion = "v1.0")
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("br"));
            client.DefaultRequestHeaders.Connection.Add("keep-alive");
            client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
            var url = FormUrl(daprAppIdOrHost, serviceMethod, serviceVersion);
            var result = await client.GetAsync(url);
            var content = await GetZipBody(result);
            return content;

        }

        public async Task<string> GetExternalContentAsync(string rawUrl)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("br"));
            client.DefaultRequestHeaders.Connection.Add("keep-alive");
            client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
            var result = await client.GetAsync(rawUrl);
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
