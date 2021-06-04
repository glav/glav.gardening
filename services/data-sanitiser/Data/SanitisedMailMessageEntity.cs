using System;
using System.Collections.Generic;
using System.Text;

namespace Glav.DataSanitiser.Data
{
    public class SanitisedDataEntity
    {
        public string FromAddresses { get; set; }
        public string ToAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SanitisedBody { get; set; }

    }

}
