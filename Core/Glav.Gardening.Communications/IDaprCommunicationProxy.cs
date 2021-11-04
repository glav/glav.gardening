using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glav.Gardening.Communications
{
    public interface IDaprCommunicationProxy
    {
        Task<string> GetContentAsync(string appId, string serviceMethod, string serviceVersion = "v1.0");
    }
}
