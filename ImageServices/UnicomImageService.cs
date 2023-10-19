using DromAutoTrader.ImageServices.Interfaces;
using DromAutoTrader.Services;
using OpenQA.Selenium;

namespace DromAutoTrader.ImageServices
{
    public class UnicomImageService : IWebsite
    {
        #region Приватный поля
        private bool IsFirstRunning = true;
        public string? ImagesLocalPath = string.Empty;
        private string? _loginPageUrl = "https://uniqom.ru/ ";
        private string? _searchPageUrl = "https://berg.ru/search/step2?search=AG19166&brand=TRIALLI&withRedirect=1";
        private string? _userName = "autobest038";
        private string? _password = "dimonfutboll";
        private IWebDriver _driver = null!;
        #endregion

        #region Публичные поля
        public string WebSiteName => "uniqom.ru";
        public string? Brand { get; set; }
        public string? Articul { get; set; }
        public List<string>? BrandImages { get; set; }        
        #endregion


        public UnicomImageService()
        {
            UndetectDriver webDriver = new();
            _driver = webDriver.GetDriver();
        }

        public Task RunAsync(string brandName, string articul)
        {
            throw new NotImplementedException();
        }

        public void Authorization()
        {
            throw new NotImplementedException();
        }

        public IWebElement GetSearchInput()
        {
            throw new NotImplementedException();
        }
               

        public void SetArticulInSearchInput()
        {
            throw new NotImplementedException();
        }

         void OpenWebsite(string url)
        {
            _driver.Navigate().GoToUrl(url);
        }

         void PerformLogin(string username, string password)
        {
            // Найти элемент для ввода логина
            IWebElement usernameElement = _driver.FindElement(By.Name("username"));

            // Ввести логин
            usernameElement.SendKeys(username);

            // Найти элемент для ввода пароля
            IWebElement passwordElement = _driver.FindElement(By.Name("password"));

            // Ввести пароль
            passwordElement.SendKeys(password);

            // Найти кнопку для входа и нажать на нее
            IWebElement loginButton = _driver.FindElement(By.CssSelector(".login__button"));
            loginButton.Click();
        }

         void SearchProduct(string searchTerm)
        {
            // Найти поле для поиска
            IWebElement searchField = _driver.FindElement(By.Id("m-header-search-l"));

            // Ввести поисковый запрос
            searchField.SendKeys(searchTerm);

            // Нажать клавишу "Enter" для поиска
            searchField.SendKeys(Keys.Enter);
        }

         void SelectProduct()
        {
            // Нажать на элемент, представляющий выбранный продукт
            IWebElement productElement = _driver.FindElement(By.CssSelector("div:nth-child(1) > div > .product__card > .card__section .svg-inline--fa"));
            productElement.Click();
        }

         void OpenProductImage()
        {
            // Нажать на элемент, представляющий открытие изображения продукта
            IWebElement imageElement = _driver.FindElement(By.CssSelector(".uk-lightbox-toolbar-icon line:nth-child(2)"));
            imageElement.Click();
        }
    }
}
