using Glav.DataSanitiser.Sanitisers.Strategies;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glav.DataSanitiser
{
    public class DataSanitiserEngine
    {
        private readonly ILogger _logger;
        private List<IDataSanitiserStrategy> _strategiesToUse;
        public DataSanitiserEngine(List<IDataSanitiserStrategy> strategiesToUse, ILogger logger)
        {
            _logger = logger;
            if (strategiesToUse == null || strategiesToUse.Count == 0)
            {
                SetupDefaultStrategies();
            } else {
                _strategiesToUse = strategiesToUse;
            }
        }
 
        private void SetupDefaultStrategies()
        { 
            _strategiesToUse = new List<IDataSanitiserStrategy>();
            _strategiesToUse.Add(new RemoveAllButBodyStrategy());
            _strategiesToUse.Add(new RemoveHtmlCommentsStrategy());
            _strategiesToUse.Add(new RemoveHtmlStrategy());
            _strategiesToUse.Add(new RemoveEncodedCharactersStrategy());
        }

        public List<IDataSanitiserStrategy> SanitiserStrategies => _strategiesToUse;

        public string SanitiseDataForAllContentTypes(string data)
        {
            _logger.LogInformation("SanitiseData");

            try
            {
                return SanitiseDataByContentType(data, SanitiseContentType.Html & SanitiseContentType.PlainText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error attempting to Sanitise Data");
            }
            return null;
        }


        public string SanitiseDataByContentType(string content, SanitiseContentType contentType)
        {
            if (_strategiesToUse.Count == 0)
            {
                return content;
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return content;
            }

            var buffer = new StringBuilder(content);
            var tmpContent = new StringBuilder();

            //bitwise enum check, can be both html or plain text
            var strategies = _strategiesToUse.Where(s => s.ContentTypeSupported.HasFlag(contentType)).ToList();
            strategies.ForEach(s =>
            {
                tmpContent.Append(s.SanitiseData(buffer.ToString()));
                buffer.Clear();
                buffer.Append(tmpContent);
                tmpContent.Clear();
            });

            return buffer.ToString();
        }
    }
}
