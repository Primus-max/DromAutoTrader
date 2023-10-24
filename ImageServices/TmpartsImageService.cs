using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Threading;
using System.Web;

namespace DromAutoTrader.ImageServices
{
    internal class TmpartsImageService : ImageServiceBase
    {

        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://tmparts.ru/";

        protected override string SearchPageUrl => "https://tmparts.ru/Lookup/FirstLook?SearchNumber=EX4854009102&laximo_cross=1";

        protected override string UserName => "ПЛ0044166";

        protected override string Password => "G97GP?ct4";

        public override string ServiceName => "tmparts.ru";
        #endregion

        public TmpartsImageService()
        {
            InitializeDriver();
        }



        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //


        #region Перезаписанные методы базового класса
        protected override void GoTo()
        {
            _driver.Manage().Window.Maximize();
            _driver.Navigate().GoToUrl(LoginPageUrl);
        }

        protected override void Authorization()
        {
            try
            {
                try
                {
                    IWebElement logInput = _driver.FindElement(By.Id("inputEmail4"));
                    Actions builder = new Actions(_driver);

                    builder.MoveToElement(logInput)
                           .Click()
                           .SendKeys(UserName)
                           .Build()
                           .Perform();

                    Thread.Sleep(500);
                }
                catch (Exception) { }

                try
                {
                    IWebElement passInput = _driver.FindElement(By.Id("inputPassword4"));

                    Thread.Sleep(200);

                    passInput.SendKeys(Password);
                }
                catch (Exception) { }

                try
                {
                    IWebElement sumbitBtn = _driver.FindElement(By.CssSelector(".btn.btn-default"));

                    sumbitBtn.Click();

                    Thread.Sleep(200);
                }
                catch (Exception) { }
            }
            catch (Exception ex)
            {
                // TODO сделать логирование
                string message = $"Произошла ошибка в методе Authorization: {ex.Message}";
                Console.WriteLine(message);
            }
        }

        protected override void SetArticulInSearchInput()
        {
            string? searchUrl = BuildUrl();

            _driver.Navigate().GoToUrl(searchUrl);
        }

        protected override bool IsNotMatchingArticul()
        {
            // <h4 class="red" style="margin-bottom: 8px;">Извините, артикул не найден!</h4>

            bool isNotMatchingArticul = false;
            try
            {
                IWebElement attentionMessage = _driver.FindElement(By.CssSelector("h4.red"));

                // Если получили этот элемент значит по запросу ничего не найдено
                return true;

            }
            catch (Exception)
            {
                return isNotMatchingArticul;
            }
        }

        protected override void OpenSearchedCard()
        {
            IWebElement searchedCard = null!;

            try
            {
                searchedCard = _driver.FindElement(By.CssSelector("div.panelpanel-default"));
                IWebElement titleCard = _driver.FindElement(By.CssSelector("h3.panel-title"));

                string titleCardText = titleCard.Text;
                if (titleCardText.Contains("Полное совпадение запроса "))
                {
                    try
                    {
                        IList<IWebElement> searchCardLink = searchedCard.FindElements(By.CssSelector("tr.tissTooltip"));
                        IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                        js.ExecuteScript("arguments[0].click();", searchCardLink);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }




            }
            catch (Exception ex)
            {
                string message = $"Произошла ошибка в методе GetSearchedCard: {ex.Message}";
                Console.WriteLine(message);
            }
        }

        protected override bool IsImagesVisible()
        {
            throw new NotImplementedException();
        }

        protected override Task<List<string>> GetImagesAsync()
        {
            throw new NotImplementedException();
        }

        protected override void SpecificRunAsync(string brandName, string articul)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Специфичные методы класса 
        // Метод для формирования Url поискового запроса
        public string BuildUrl()
        {
            var uri = new Uri(SearchPageUrl);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query["SearchNumber"] = Articul;

            // Построение нового URL с обновленными параметрами
            string newUrl = uri.GetLeftPart(UriPartial.Path) + "?" + query.ToString();

            return newUrl;
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
