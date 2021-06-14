using System;
using System.Collections.Generic;
using System.Text;

namespace Glav.DataSanitiser.Sanitisers.Strategies
{
    public interface IDataSanitiserStrategy
    {
        SanitiseContentType ContentTypeSupported { get; }
        string SanitiseData(string content);
    }
}
