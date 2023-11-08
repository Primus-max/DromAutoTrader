using OpenQA.Selenium;
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

        public UndetectDriver(string profilePath) => _profilePath = profilePath;

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

            //var chromeService = ChromeDriverService.CreateDefaultService();
            //chromeService.HideCommandPromptWindow = true;

            IWebDriver driver = null!;

            try
            {
                driver = UndetectedChromeDriver.Create(options: options, hideCommandPromptWindow: true, driverExecutablePath: "chromedriver.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось получить драйвер, причина: {ex.ToString()}");
            }

            return driver;
        }
    }
}
