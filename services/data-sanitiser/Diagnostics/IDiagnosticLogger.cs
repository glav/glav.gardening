using System;

namespace Glav.DataSanitiser.Diagnostics
{
    public interface IDiagnosticLogger
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message);

        void Info(string messageTemplate, params object[] args);
        void Warning(string messageTemplate, params object[] args);
        void Error(string messageTemplate, params object[] args);

        void Info(Exception ex, string messageTemplate, params object[] args);
        void Warning(Exception ex, string messageTemplate, params object[] args);
        void Error(Exception ex, string messageTemplate, params object[] args);


    }
}
