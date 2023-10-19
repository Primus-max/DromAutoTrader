using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Threading;
using System.Web;

namespace DromAutoTrader.ImageServices
{
    /// <summary>
    /// Класс для получения изображений деталей брэндов с сайта https://berg.ru/ 
    /// </summary>
    internal class BergImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://berg.ru/login";

        protected override string SearchPageUrl => "https://berg.ru/search/step2?search=AG19166&brand=TRIALLI&withRedirect=1";

        protected override string UserName => "autobest038";

        protected override string Password => "dimonfutboll";

        public override string ServiceName => "berg.ru";
        #endregion

        #region Приватный поля        
        public string? _imagesLocalPath = string.Empty;
        protected IWebDriver _driver = null!;
        #endregion


        public BergImageService()
        {
            InitializeDriver();
        }

        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //


        #region Перезаписанные методы базового класса
        // TODO удалить из базового класса если не пригодится
        protected override void SpecificRunAsync(string brandName, string articul) { }

        // Метод перехода по ссылке
        protected override void GoTo()
        {
            _driver.Manage().Window.Maximize();
            _driver.Navigate().GoToUrl(LoginPageUrl);
        }

        // Метод авторизации
        protected override void Authorization()
        {
            try
            {
                try
                {
                    IWebElement logInput = _driver.FindElement(By.Id("username"));
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
                    IWebElement passInput = _driver.FindElement(By.Id("password"));

                    Thread.Sleep(200);
                    //ClearAndEnterText(passInput, _password);
                    passInput.SendKeys(Password);
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

        // Метод отправки поискового запроса
        protected override void SetArticulInSearchInput()
        {
            string? searchUrl = BuildUrl();

            _driver.Navigate().GoToUrl(searchUrl);
        }

        // Метод открытия каротчки с полученным запросом
        protected override void OpenSearchedCard()
        {
            try
            {
                IWebElement searchedCard = _driver.FindElement(By.CssSelector(".search_result__row.first_row"));
                IWebElement searchCardLink = searchedCard.FindElement(By.CssSelector(".pseudo_link.part_description__link"));

                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                js.ExecuteScript("arguments[0].click();", searchCardLink);
            }
            catch (Exception ex)
            {
                string message = $"Произошла ошибка в методе GetSearchedCard: {ex.Message}";
                Console.WriteLine(message);
            }
        }

        // Метод проверки, появились картинки или нет
        protected override bool IsImagesVisible()
        {
            bool isVisible = false;
            int tryCount = 0;

            while (!false)
            {
                try
                {
                    IWebElement imagePreview = _driver.FindElement(By.ClassName("preview_img__container"));

                    isVisible = imagePreview != null;

                    return isVisible;
                }
                catch (Exception)
                {
                    tryCount++;

                    if (tryCount == 20)
                    {
                        break;
                    }

                    Thread.Sleep(500);

                    continue;
                }
            }
            return isVisible;
        }

        // Метод проверки результатов поиска детали
        protected override bool IsNotMatchingArticul()
        {
            bool isNotMatchingArticul = false;
            try
            {
                IWebElement attentionMessage = _driver.FindElement(By.ClassName("attention_message"));

                // Если получили этот элемент значит по запросу ничего не найдено
                return true;

            }
            catch (Exception)
            {
                return isNotMatchingArticul;
            }
        }

        // Метод сбора картинок из открытой карточки
        protected override async Task<List<string>> GetImagesAsync()
        {
            // Список изображений которые возвращаем из метода
            List<string> downloadedImages = new List<string>();

            // Временное хранилище изображений
            List<string> images = new List<string>();
            IWebElement mainImageParentDiv = null!;

            // Получаю контейнер с картинками
            try
            {
                mainImageParentDiv = _driver.FindElement(By.ClassName("photo_gallery"));
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


            // Скачиваю изображения
            ImageDownloader? downloader = new(Articul, _imagesLocalPath, images);
            downloadedImages = await downloader.DownloadImagesAsync();

            return downloadedImages;
        }

        #endregion

        #region Специфичные методы класса       

        // Метод для формирования Url поискового запроса
        public string BuildUrl()
        {
            var uri = new Uri(SearchPageUrl);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query["search"] = Articul;
            query["brand"] = Brand;

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
