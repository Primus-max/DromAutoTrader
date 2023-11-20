using OpenQA.Selenium.Chrome;
using SeleniumUndetectedChromeDriver;

namespace DromAutoTrader.Services
{
    /// <summary>
    /// Класс инициализации WebDriver'а 
    /// </summary>
    class UndetectDriver
    {
        private readonly string _profilePath = string.Empty;
        private readonly Logger _logger;
        public UndetectDriver(string profilePath) 
        {
            _profilePath = profilePath;
            _logger = new LoggingService().ConfigureLogger();
        }

        /// <summary>
        /// Метод инициализирует экземпляр драйвера и возвращает его
        /// </summary>
        /// <returns>IWebDriver</returns>
        public IWebDriver GetDriver()
        {
            ChromeOptions options = new();
            options.AddArgument("--silent");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--headless=new");
            options.AddArgument($"--user-data-dir={_profilePath}");

            // Отключение расширений
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-extensions-file-access-check");
            options.AddArgument("--disable-extensions-http-throttling");

            //var chromeService = ChromeDriverService.CreateDefaultService();
            //chromeService.HideCommandPromptWindow = true;

            IWebDriver driver = null!;

            try
            {
                driver = UndetectedChromeDriver.Create(options: options, hideCommandPromptWindow: true, driverExecutablePath: "chromedriver.exe", commandTimeout: TimeSpan.FromSeconds(15));
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при создании драйвер в методе GetDriver: {ex.Message}");
            }

            // Глобальное ожидание
            if (driver != null)
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
            }

            return driver;
        }
    }
}
