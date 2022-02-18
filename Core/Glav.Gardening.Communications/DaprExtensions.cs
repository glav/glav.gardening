using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Glav.Gardening.Communications
{
    public static class DaprExtensions
    {
        public static Task<string> StoreState(this ICommunicationProxy proxy, string key, object data, string serviceVersion = "v1.0")
        {
            var jsonData = System.Text.Json.JsonSerializer.Serialize(data);
            var storeData = "[ {\"key\":\"" + key + "\", \"value\":" + jsonData + " } ]";

            return proxy.PostContentAsync(ServiceAppId.State, "statestore", new StringContent(storeData,Encoding.UTF8,"application/json"), serviceVersion);
        }

        public static Task<string> GetState(this ICommunicationProxy proxy, string key, string serviceVersion = "v1.0")
        {
            return proxy.GetContentAsync(ServiceAppId.State,$"statestore/{key}", serviceVersion);
        }
    }
}
