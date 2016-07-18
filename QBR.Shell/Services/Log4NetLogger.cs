using log4net;
using Microsoft.Practices.Prism.Logging;

namespace QBR.Shell.Services
{
    class Log4NetLogger : ILoggerFacade
    {
        //Get the specific logger for Log4NetLogger Class. By default, we just use the root logger.
        private readonly ILog _log4NetLogger = LogManager.GetLogger(typeof (Log4NetLogger));

        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    _log4NetLogger.Debug(message);
                    break;
                case Category.Warn:
                    _log4NetLogger.Warn(message);
                    break;
                case Category.Exception:
                    _log4NetLogger.Error(message);
                    break;
                case Category.Info:
                    _log4NetLogger.Info(message);
                    break;
            }
        }
    }
}
