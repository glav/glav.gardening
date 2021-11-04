using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glav.InformationGathering.Utilities
{
    public static class UrlHelper
    {
        public static string CreateDaprServiceUrl(string appId, string method)
        {
            return $"http://localhost:3500/v1.0/invoke/{appId}/method/{method}";
        }
    }
}
