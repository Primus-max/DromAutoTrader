
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace DromAutoTrader.ImageServices
{
    /// <summary>
    /// Класс для получения изображений деталей брэндов с сайта https://berg.ru/ 
    /// </summary>
    internal class BergImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://berg.ru/login";

        protected override string SearchPageUrl => "https://berg.ru/search?search=";

        protected override string UserName => "autobest038";

        protected override string Password => "dimonfutboll";

        public override string ServiceName => "https://berg.ru";
        #endregion

        #region Приватный поля        
        private readonly string _profilePath = @"C:\SeleniumProfiles\Berg";
        private string _tempProfilePath = string.Empty;
        private IHtmlDocument _document = null!;
        #endregion        

        public BergImageService()
        {
            // Создаю временную копию профиля (на эту сессию)
            ProfilePathService profilePathService = new();
            _tempProfilePath = profilePathService.CreateTempProfile(_profilePath);

            //InitializeDriver();
        }

        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //


        #region Перезаписанные методы базового класса  

        // Метод перехода по ссылке
        protected override void GoTo()
        {
            Task.Run(async () => await GoToAsync()).Wait();
        }


        // Метод авторизации
        protected override void Authorization()
        {

        }

        // Метод отправки поискового запроса
        protected override void SetArticulInSearchInput()
        {
            string? searchUrl = BuildUrl();

            try
            {
                _driver.Navigate().GoToUrl(searchUrl);
            }
            catch (Exception)
            {

            }
        }

        // Метод открытия каротчки с полученным запросом
        protected override void OpenSearchedCard()
        {
            WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(7));
            try
            {
                // Получаю все карточки на странице
                IList<IWebElement> searchedCards = wait.Until(e => e.FindElements(By.CssSelector("div.search_result__row")));

                foreach (var card in searchedCards)
                {
                    // Получаю бренд и артикул из карточек
                    string brand = card.FindElement(By.ClassName("brand_name")).Text.ToLower().Replace(" ", "");
                    string articul = card.FindElement(By.XPath("//div[@class='article']/a[1]")).Text;
                    string? globalNameBrand = Brand.ToLower().Replace(" ", "");

                    // Если совпадает открываю Popup
                    if (globalNameBrand == brand && Articul == articul)
                    {
                        IWebElement popUp = card.FindElement(By.CssSelector("a.pseudo_link.part_description__link"));
                        // Получаю ссылку на Popup
                        string popupUrl = popUp.GetAttribute("href");
                        string popUpLink = $"{ServiceName}{popupUrl}";

                        try
                        {
                            _driver.Navigate().GoToUrl(popupUrl);
                        }
                        catch (Exception) { }

                    }
                }

            }
            catch (Exception)
            {

            }
        }

        // Метод проверки, появились картинки или нет
        protected override bool IsImagesVisible()
        {
            WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(3));

            try
            {
                IWebElement mainImageParentDiv = wait.Until(e => e.FindElement(By.ClassName("photo_gallery")));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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

            if (images.Count != 0)
                downloadedImages = await ImagesProcessAsync(images);

            return downloadedImages;
        }

        // Метод создания директории и скачивания изображений
        private async Task<List<string>> ImagesProcessAsync(List<string> images)
        {
            List<string> downloadedImages = new();

            // Проверяю создан ли путь для хранения картинок
            FolderManager folderManager = new();
            bool folderContainsFiles = folderManager.ArticulFolderContainsFiles(brand: Brand, articul: Articul, out _imagesLocalPath);

            await Task.Delay(1000);

            if (!folderContainsFiles)
            {
                // Скачиваю изображения
                ImageDownloader? downloader = new(Articul, _imagesLocalPath, images);
                downloadedImages = await downloader.DownloadImagesAsync();
            }

            return downloadedImages;
        }

        protected override async Task CloseDriverAsync()
        {
            try
            {
                _driver.Close();
                _driver.Quit();
                _driver.Dispose();

                // Удаляю временную директорию профиля после закрытия браузера
                ProfilePathService profilePathService = new();
                await profilePathService.DeleteDirectoryAsync(_tempProfilePath);
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Специфичные методы класса    
        // Асинхронный метод получения DOM
        protected async Task GoToAsync()
        {
            try
            {
                // Создаем объект HttpClient
                using var httpClient = new HttpClient();
                // Формируем URL для запроса
                var searchUrl = BuildUrl(); // замените "your_keyword" на фактическое значение

                // Создаем контейнер для хранения кук
                var cookieContainer = new System.Net.CookieContainer();

                // Устанавливаем куки (замените значения на актуальные)
                cookieContainer.Add(new Uri(ServiceName), new System.Net.Cookie("BERG_SESSID", "b39b60eadfac0d2fc91d7f8acf45687e"));

                // Создаем объект HttpClientHandler и передаем ему контейнер с куками
                var handler = new HttpClientHandler { CookieContainer = cookieContainer };

                // Создаем новый HttpClient с настроенным обработчиком
                using var httpClientWithCookies = new HttpClient(handler);
                // Отправляем GET-запрос и получаем ответ
                var response = await httpClientWithCookies.GetAsync(searchUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Получаем документ
                    var pageSource = await response.Content.ReadAsStringAsync();
                    var context = BrowsingContext.New(Configuration.Default);
                    var parser = context.GetService<IHtmlParser>();
                    _document = parser?.ParseDocument(pageSource);
                }
                else
                {
                    Console.WriteLine($"Не удалось получить DOM для Berg {response.Headers}");
                    // TODO: сделать логирование
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обращении к Berg {ex.Message}");
            }
        }

        // Метод для формирования Url поискового запроса
        public string BuildUrl()
        {
            // Построение нового URL с обновленными параметрами
            string newUrl = SearchPageUrl + Articul;

            return newUrl;
        }

        // Инициализация драйвера
        private void InitializeDriver()
        {
            UndetectDriver webDriver = new(_tempProfilePath);
            _driver = webDriver.GetDriver();
        }

        #endregion
    }
}
