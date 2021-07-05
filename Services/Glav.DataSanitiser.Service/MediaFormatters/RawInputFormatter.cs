using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Glav.DataSanitiser.MediaFormatters
{
    public class RawInputFormatter : InputFormatter
    {
        public const string TextPlainMediaType = "text/plain";
        public const string ApplicationOctetMediaType = "application/octet-stream";
        public RawInputFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(TextPlainMediaType));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(ApplicationOctetMediaType));
        }


        public override Boolean CanRead(InputFormatterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var contentType = context.HttpContext.Request.ContentType;
            return string.IsNullOrEmpty(contentType)
                    || contentType == TextPlainMediaType
                    || contentType == ApplicationOctetMediaType;
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            var contentType = context.HttpContext.Request.ContentType;


            if (string.IsNullOrEmpty(contentType) || contentType == TextPlainMediaType)
            {
                using (var reader = new StreamReader(request.Body))
                {
                    var content = await reader.ReadToEndAsync();
                    return await InputFormatterResult.SuccessAsync(content);
                }
            }
            if (contentType == ApplicationOctetMediaType)
            {
                using (var ms = new MemoryStream(2048))
                {
                    await request.Body.CopyToAsync(ms);
                    var content = ms.ToArray();
                    return await InputFormatterResult.SuccessAsync(content);
                }
            }

            return await InputFormatterResult.FailureAsync();
        }
    }
}
