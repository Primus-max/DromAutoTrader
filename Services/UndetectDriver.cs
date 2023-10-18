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
        public UndetectDriver() { }

        /// <summary>
        /// Метод инициализирует экземпляр драйвера и возвращает его
        /// </summary>
        /// <returns>IWebDriver</returns>
        public  IWebDriver GetDriver()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--disable-notifications");
            IWebDriver driver = null!;

            try
            {
                driver = UndetectedChromeDriver.Create(options: options, driverExecutablePath: "chromedriver.exe");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось получить драйвер, причина: {ex.Message}");
            }

            return driver;
        }
    }
}
