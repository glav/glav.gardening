using System;
using System.Collections.Generic;
using System.Text;

namespace Glav.DataSanitiser
{
    [Flags]
    public enum SanitiseContentType : Byte
    {
        PlainText,
        Html
    }
}
