using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Glav.Gardening.Communications
{
    public static class HttpExtensions
    {
        public static CompressionType GetCompressionType(this HttpResponseMessage rspMsg)
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
    }
}
