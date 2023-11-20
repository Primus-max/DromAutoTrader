using Serilog;
using Serilog.Events;

namespace DromAutoTrader.Services
{
    public class LoggingService
    {
        private static readonly object lockObject = new object();

        public Logger ConfigureLogger()
        {
            lock (lockObject)
            {
                // Создаем конфигурацию для логирования
                var loggerConfiguration = new LoggerConfiguration()
                    .WriteTo.Logger(lc => lc
                        .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Information)
                        .WriteTo.File("logs/info.txt"))
                    .WriteTo.Logger(lc => lc
                        .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Error)
                        .WriteTo.File("logs/error.txt"));

                // Создаем логгер
                var logger = loggerConfiguration.CreateLogger();

                // Устанавливаем глобальный логгер
                Log.Logger = logger;

                return logger;
            }
        }
    }
}