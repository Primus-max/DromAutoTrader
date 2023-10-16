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

        private string? _imagesLocalPath = string.Empty;
        private string? _loginPageUrl = "https://berg.ru/login";
        private string? _searchPageUrl = "https://berg.ru/search/step2?search=AG19166&brand=TRIALLI&withRedirect=1";
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

            OpenSearchedCard();

            GetImages();
        }

        // Метод авторизации
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
                string message = $"Произошла ошибка в методе Authorization: {ex.Message}";
                Console.WriteLine(message);
            }
        }

        public IWebElement GetSearchInput()
        {
            throw new NotImplementedException();
        }

        // Метод отправки поискового запроса
        public void SetArticulInSearchInput()
        {
            string? searchUrl = BuildUrl();

            _driver.Navigate().GoToUrl(searchUrl);
        }

        // Метод открытия каротчки с полученным запросом
        private void OpenSearchedCard()
        {
            try
            {
                IWebElement searchedCard = _driver.FindElement(By.ClassName("search_result__row first_row"));

                searchedCard.Click();
            }
            catch (Exception ex)
            {
                string message = $"Произошла ошибка в методе GetSearchedCard: {ex.Message}";
                Console.WriteLine(message);
            }
        }

        // Метод сбора картинок из открытой карточки
        private void GetImages()
        {
            List<string> images = new List<string>();
            IWebElement mainImageParentDiv = null!;

            // Получаю контейнер с картинками
            try
            {
                mainImageParentDiv = _driver.FindElement(By.ClassName("photo_gallery"));
            }
            catch (Exception) { }

            // Получаю картинку preview
            try
            {
                IWebElement imagePreview = mainImageParentDiv.FindElement(By.ClassName("preview_img__container"));
                string imagePath = imagePreview.GetAttribute("href");
                images.Add(imagePath);
            }
            catch (Exception) { }

            // Получаю все картинки thumbs
            try
            {
                // Находим все img элементы в li элементах с data-type='thumb'
                IList<IWebElement> imagesThumb = mainImageParentDiv.FindElements(By.XPath("//li[@data-type='thumb']/img"));

                foreach (var image in imagesThumb)
                {
                    string imagePath = image.GetAttribute("src");
                    images.Add(imagePath);
                }   
            }
            catch (Exception) { }

            // Проверяю создан ли путь для хранения картинок
            FolderManager folderManager = new FolderManager();
            folderManager.ArticulFolderContainsFiles(brand: Brand, articul: Articul, out _imagesLocalPath);


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
        public string BuildUrl()
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
