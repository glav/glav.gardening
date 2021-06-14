using System;

namespace Glav.DataSanitiser.Diagnostics
{
    public class ConsoleDiagnosticLogger : IDiagnosticLogger
    {
        private readonly ConsoleColor _originalColour;

        private const ConsoleColor _errorColour = ConsoleColor.Red;
        private const ConsoleColor _warningColour = ConsoleColor.Yellow;
        private const ConsoleColor _infoColour = ConsoleColor.White;

        public ConsoleDiagnosticLogger()
        {
            _originalColour = Console.ForegroundColor;
        }
        public bool IsHostedInAzure()
        {
            return !string.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME", EnvironmentVariableTarget.Process));
        }

        private void WriteToConsole(ConsoleColor colour, string msg)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(msg);
            Console.ForegroundColor = _originalColour;
        }


        public void Info(string message)
        {
            WriteToConsole(_infoColour,message);
        }

        public void Info(string messageTemplate, params object[] args)
        {
            Info(string.Format(messageTemplate, args));
        }
        public void Info(Exception ex, string messageTemplate, params object[] args)
        {
            Error($"{ExtractExceptionDetail(ex)} {string.Format(messageTemplate, args)}");
        }


        public void Warning(string message)
        {
            WriteToConsole(_warningColour,message);
        }

        public void Warning(string messageTemplate, params object[] args)
        {
            Warning(string.Format(messageTemplate, args));
        }

        public void Warning(Exception ex, string messageTemplate, params object[] args)
        {
            Error($"{ExtractExceptionDetail(ex)} {string.Format(messageTemplate, args)}");
        }

        public void Error(string message)
        {
            WriteToConsole(_errorColour,message);
        }

        public void Error(string messageTemplate, params object[] args)
        {
            Error(string.Format(messageTemplate,args));
        }

        public void Error(Exception ex, string messageTemplate, params object[] args)
        {
            Error($"{ExtractExceptionDetail(ex)} {string.Format(messageTemplate, args)}");
        }


        private string ExtractExceptionDetail(Exception ex)
        {
            return $"[{ex.Message}]";
        }
    }
}