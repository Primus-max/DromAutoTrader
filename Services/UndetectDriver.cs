using OpenQA.Selenium;
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
            var driver = UndetectedChromeDriver.Create(driverExecutablePath: "chromedriver.exe");

            return driver;
        }
    }
}
