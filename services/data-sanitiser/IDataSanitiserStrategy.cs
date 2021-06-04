using System;
using System.Collections.Generic;
using System.Text;

namespace Glav.DataSanitiser
{
    public interface IDataSanitiserStrategy
    {
        SanitiseContentType ContentTypeSupported { get; }
        string SanitiseData(string content);
    }
}
