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
        private const string DEFAULT_STATESTORE_NAME = "statestore";
        private const string DEFAULT_PUBSUB_NAME = "pubsub";
        public static Task<string> StoreState(this ICommunicationProxy proxy, string key, object data, string serviceVersion = "v1.0")
        {
            var jsonData = System.Text.Json.JsonSerializer.Serialize(data);
            // Limitation here as we are only adding 1 element/key value to the state store but it supports multiple key values
            var storeData = "[ {\"key\":\"" + key + "\", \"value\":" + jsonData + " } ]";

            return proxy.PostContentAsync(ServiceAppId.State, DEFAULT_STATESTORE_NAME, new StringContent(storeData,Encoding.UTF8,"application/json"), serviceVersion);
        }

        public static Task<string> GetState(this ICommunicationProxy proxy, string key, string serviceVersion = "v1.0")
        {
            return proxy.GetContentAsync(ServiceAppId.State,$"{DEFAULT_STATESTORE_NAME}/{key}", serviceVersion);
        }
        public static Task<string> Publish(this ICommunicationProxy proxy, string topic, object eventData, string serviceVersion = "v1.0")
        {
            var jsonData = System.Text.Json.JsonSerializer.Serialize(eventData);

            return proxy.PostContentAsync(ServiceAppId.PubSub, $"{DEFAULT_PUBSUB_NAME}/{topic}", new StringContent(jsonData,Encoding.UTF8,"application/json"), serviceVersion);
        }
    }
}
