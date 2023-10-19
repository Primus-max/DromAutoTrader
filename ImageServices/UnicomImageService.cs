using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using System.Threading;

namespace DromAutoTrader.ImageServices
{
    public class UnicomImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://uniqom.ru/";

        protected override string SearchPageUrl => LoginPageUrl;

        protected override string UserName => "autobest038";

        protected override string Password => "dimonfutboll";

        public override string ServiceName => "uniqom.ru";
        #endregion

        #region Приватный поля
        //private bool _isFirstRunning = true;
        public string? _imagesLocalPath = string.Empty;
        protected IWebDriver _driver = null!;
        #endregion

        #region Публичные поля        
        public string? Brand { get; set; }
        public string? Articul { get; set; }
        public List<string>? BrandImages { get; set; }
        #endregion

        public UnicomImageService()
        {
            InitializeDriver();
        }

        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //


        #region Перезаписанные методы базового класса
        protected override void SpecificRunAsync(string brandName, string articul)
        {
            throw new NotImplementedException();
        }

        // Метод перехода по ссылке
        protected override void GoTo()
        {
            _driver.Manage().Window.Maximize();
            _driver.Navigate().GoToUrl(LoginPageUrl);
        }

        protected override void Authorization()
        {

            try
            {
                // Поле для ввода логина
                IWebElement usernameElement = _driver.FindElement(By.Name("username"));
                Thread.Sleep(200);
                // Ввести логин
                usernameElement.SendKeys(UserName);
            }
            catch (Exception) { }

            try
            {
                // Поле для ввода пароля
                IWebElement passwordElement = _driver.FindElement(By.Name("password"));
                Thread.Sleep(200);
                // Ввести пароль
                passwordElement.SendKeys(Password);
            }
            catch (Exception) { }

            try
            {
                // Кнопка для входа и нажать на нее
                IWebElement loginButton = _driver.FindElement(By.CssSelector(".login__button"));
                loginButton.Click();
            }
            catch (Exception) { }
        }

        protected override void SetArticulInSearchInput()
        {
            try
            {
                // Найти поле для поиска
                IWebElement searchField = _driver.FindElement(By.Id("m-header-search-l"));

                // Ввести поисковый запрос
                searchField.SendKeys(Articul);

                // Нажать клавишу "Enter" для поиска
                searchField.SendKeys(Keys.Enter);
            }
            catch (Exception) { }
        }

        protected override bool IsNotMatchingArticul()
        {
            throw new NotImplementedException();
        }

        protected override void OpenSearchedCard()
        {
            throw new NotImplementedException();
        }

        protected override bool IsImagesVisible()
        {
            throw new NotImplementedException();
        }

        protected override Task<List<string>> GetImages()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Специфичные методы класса   
        void PerformLogin(string username, string password)
        {

        }

        void SearchProduct(string searchTerm)
        {

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

        // Инициализация драйвера
        private void InitializeDriver()
        {
            UndetectDriver webDriver = new();
            _driver = webDriver.GetDriver();
        }
        #endregion
    }
}
