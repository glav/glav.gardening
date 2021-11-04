using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Glav.Gardening.Communications
{
    public class DaprCommunicationProxy : IDaprCommunicationProxy
    {
        public async Task<string> GetContentAsync(string appId, string serviceMethod, string serviceVersion = "v1.0")
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.Connection.Add("keep-alive");
            client.DefaultRequestHeaders.Add("User-Agent", "query/agent");
            var result = await client.GetAsync(FormUrl(appId,serviceMethod,serviceVersion));
            return await result.Content.ReadAsStringAsync();
        }

        private static string FormUrl(string appId, string serviceMethod, string serviceVersion = "v1.0")
        {
            var port = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
            if (string.IsNullOrWhiteSpace(port))
            {
                throw new Exception("Not running in a DAPR instance - no DAPR_HTTP_PORT defined");
            }
            var daprEndpoint = $"localhost:{port}/{serviceVersion}/invoke/{appId}/method/{serviceMethod}";
            return daprEndpoint;

        }
    }
}
