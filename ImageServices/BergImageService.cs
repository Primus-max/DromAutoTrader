using DromAutoTrader.ImageServices.Interfaces;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using System.Threading;
using System.Web;

namespace DromAutoTrader.ImageServices
{
    /// <summary>
    /// Класс для получения изображений деталей брэндов с сайта https://berg.ru/ 
    /// </summary>
    class BergImageService : IWebsite
    {

        public string WebSiteName => "berg.ru";
        public string? Brand { get; set; }
        public string? Articul { get; set; }
        public List<string>? BrandImages { get; set; }

        private string? _loginPageUrl = "https://berg.ru/login";
        private string ? _searchPageUrl = "https://berg.ru/search/step2?search=AG19166&brand=TRIALLI&withRedirect=1";
        private string? _userName = "autobest038";
        private string? _password = "dimonfutboll";

        private IWebDriver _driver = null!;


        public BergImageService(string brandName, string articul)
        {
            Brand = brandName;
            Articul = articul;

            UndetectDriver webDriver = new();
            _driver = webDriver.GetDriver();


        }

        #region Методы
        // Метод-точка вход 
        public void Run()
        {
            _driver.Manage().Window.Maximize();
            _driver.Navigate().GoToUrl(_loginPageUrl);

            Authorization();

            SetArticulInSearchInput();
        }

        public void Authorization()
        {
            try
            {
                try
                {
                    IWebElement logInput = _driver.FindElement(By.Id("username"));

                    logInput.SendKeys(_userName);

                    Thread.Sleep(200);
                }
                catch (Exception) { }

                try
                {
                    IWebElement passInput = _driver.FindElement(By.Id("password"));

                    passInput.SendKeys(_password);

                    Thread.Sleep(200);
                }
                catch (Exception) { }

                try
                {
                    IWebElement sumbitBtn = _driver.FindElement(By.Id("_submit"));

                    sumbitBtn.Click();

                    Thread.Sleep(200);
                }
                catch (Exception) { }
            }
            catch (Exception ex)
            {
                // TODO сделать логирование
                string message = $"Произошла ошибка {ex.Message}";
            }
        }

        public IWebElement GetSearchInput()
        {
            throw new NotImplementedException();
        }

        public void SetArticulInSearchInput()
        {
            string? searchUrl =  BuildeUrl();

            _driver.Navigate().GoToUrl(searchUrl);
           
        }

        // Метод отчистки полей и вставки текста
        private static void ClearAndEnterText(IWebElement element, string text)
        {
            Random random = new Random();
            // Используем JavaScriptExecutor для выполнения JavaScript-кода
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)((IWrapsDriver)element).WrappedDriver;

            // Очищаем поле ввода с помощью JavaScript
            jsExecutor.ExecuteScript("arguments[0].value = '';", element);
            // Установить стиль display элемента в block
            jsExecutor.ExecuteScript("arguments[0].style.display = 'block';", element);
            // Вставляем текст по одному символу без изменений
            foreach (char letter in text)
            {
                if (letter == '\b')
                {
                    // Если символ является символом backspace, удаляем последний введенный символ
                    element.SendKeys(Keys.Backspace);
                }
                else
                {
                    // Вводим символ
                    element.SendKeys(letter.ToString());
                }

                Thread.Sleep(random.Next(50, 150));  // Добавляем небольшую паузу между вводом каждого символа
            }
            Thread.Sleep(random.Next(300, 700));
        }

        // Метод для формирования Url поискового запроса
        public string BuildeUrl()
        {
            var uri = new Uri(_searchPageUrl);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query["search"] = Articul;
            query["brand"] = Brand;

            // Построение нового URL с обновленными параметрами
            string newUrl = uri.GetLeftPart(UriPartial.Path) + "?" + query.ToString();

            return newUrl;
        }
        #endregion

    }
}
