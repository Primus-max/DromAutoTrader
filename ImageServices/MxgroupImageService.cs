namespace DromAutoTrader.ImageServices
{
    public class MxgroupImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://new.mxgroup.ru/#login-client";

        protected override string SearchPageUrl => "https://new.mxgroup.ru/b/search/n/";

        protected override string UserName => "krasikov98975rus@gmail.com";

        protected override string Password => "H2A8AMZ757";

        public override string ServiceName => "https://new.mxgroup.ru";
        #endregion

        #region Приватные        
        private readonly string _profilePath = @"C:\SeleniumProfiles\Mxgroup";
        private string _tempProfilePath = string.Empty;
        #endregion

        public MxgroupImageService()
        {

            // Создаю временную копию профиля (на эту сессию)
            ProfilePathService profilePathService = new();
            _tempProfilePath = profilePathService.CreateTempProfile(_profilePath);

            InitializeDriver();

        }

        // Передаю документ из Selenium в AngleSharp
        protected IHtmlDocument GetHtmlDocument()
        {
            return (IHtmlDocument)BrowsingContext.New(Configuration.Default)
                .OpenAsync(req => req.Content(_driver.PageSource)).Result;
        }


        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //


        #region Перезаписанные методы базового класса       
        protected override void GoTo()
        {
            try
            {
                _driver.Navigate().GoToUrl(ServiceName);
            }
            catch (Exception)
            {
            }
        }

        protected override void Authorization()
        {
            WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(20));
            try
            {
                IWebElement authBtn = wait.Until(e => e.FindElement(By.CssSelector("button.btn")));

                Thread.Sleep(1000);
                authBtn.Submit();
            }
            catch (Exception)
            {
                var asdf = "";
            }
        }

        protected override void SetArticulInSearchInput()
        {
            // Проверка на наличие спиннеров свидетельствующих о загрузке страницы
            bool isSpinner = true;
            while (isSpinner)
            {
                Thread.Sleep(500);
                try
                {
                    IWebElement spinner1 = _driver.FindElement(By.CssSelector(".icon.spinner.mx-spin"));

                    try
                    {
                        IWebElement spinner12 = _driver.FindElement(By.CssSelector(".indicator-wrapper.disabled"));
                        break;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                catch (Exception)
                {
                    isSpinner = false;
                }
            }

            // Если страница загружена, то перехожу к поиску
            if (!isSpinner)
            {
                string? searchUrl = BuildUrl();

                try
                {
                    _driver.Navigate().GoToUrl(searchUrl);
                }
                catch (Exception) { }
            }
        }

        protected override bool IsNotMatchingArticul()
        {
            bool isNotMatchingArticul = false;
            try
            {
                IWebElement attentionMessage = _driver.FindElement(By.ClassName("mx-text_secondary"));

                string warningTest = attentionMessage.Text;

                if (!string.IsNullOrEmpty(warningTest))
                    return true;

            }
            catch (Exception)
            {
                return isNotMatchingArticul;
            }
            return isNotMatchingArticul;
        }

        protected override void OpenSearchedCard()
        {

        }

        protected override bool IsImagesVisible()
        {
            return true;
        }

        protected override async Task<List<string>> GetImagesAsync()
        {
            // Еще одна проверка на загрузку страницы
            bool isSpinner = true;
            int tryCount = 0;
            while (isSpinner)
            {
                tryCount++;
                if (tryCount == 500) break;
                //class="indicator-wrapper disabled"
                Thread.Sleep(500);
                try
                {
                    IWebElement spinner2 = _driver.FindElement(By.CssSelector(".indicator-wrapper.enabled"));


                    //IWebElement spinner22 = _driver.FindElement(By.CssSelector(".indicator-wrapper.disabled"));
                    continue;
                }
                catch (Exception)
                {
                    break;
                }
            }

            // Список изображений которые возвращаем из метода
            List<string> downloadedImages = new List<string>();

            // Временное хранилище изображений
            List<string> images = new List<string>();
            IWebElement mainImageParentUl = null!;
            IWebElement imgPopup = null!;

            // Если открылась другая таблица, то по другому забираем фото
            string offerLink = _driver.Url;
            if (offerLink.Contains("offers"))
            {
                downloadedImages = await GetImagesFromOtherSourceAsync();

                if (downloadedImages.Count > 0)
                    return downloadedImages;
            }

            // Получаю контейнер с картинками
            try
            {
                mainImageParentUl = _driver.FindElement(By.CssSelector("ul.mx-sr-item__products"));
            }
            catch (Exception) { }

            // Получаю все картинки thumbs
            try
            {
                // Находим все img элементы в li элементах с data-type='thumb'
                IList<IWebElement>? itemProductsLi = mainImageParentUl?.FindElements(By.TagName("li"));

                IWebElement? itemProductIcon = itemProductsLi[0]?.FindElement(By.ClassName("mx-sr-item__img"));


                if (itemProductIcon is not null)
                {
                    itemProductIcon.Click();
                    await Task.Delay(1000);

                    try
                    {
                        imgPopup = _driver.FindElement(By.CssSelector("div.slider__item.img-viewer__slider-item img"));
                    }
                    catch (Exception)
                    {

                    }

                    string imagePath = imgPopup.GetAttribute("src");
                    images.Add(imagePath);
                }
            }
            catch (Exception)
            {

            }

            if (images.Count != 0)
                downloadedImages = await ImagesProcessAsync(images);

            return downloadedImages;
        }

        protected override async Task CloseDriverAsync()
        {
            try
            {
                _driver.Close();
                _driver.Quit();
            }
            catch (Exception)
            {
                // Обработка ошибок при закрытии браузера
            }
            finally
            {
                try
                {
                    _driver.Dispose();
                }
                catch (Exception)
                {
                    // Обработка ошибок при освобождении ресурсов
                }
            }

            await Task.Delay(2000);

            // Удаляю временную директорию профиля после закрытия браузера
            ProfilePathService profilePathService = new();
            await profilePathService.DeleteDirectoryAsync(_tempProfilePath);
        }
        #endregion


        #region Специфичные методы класса    
        // Метод на случай если появилась другая таблица, в ней другие селекторы
        private async Task<List<string>> GetImagesFromOtherSourceAsync()
        {
            // Список изображений которые возвращаем из метода
            List<string> downloadedImages = new List<string>();

            // Временное хранилище изображений
            List<string> images = new List<string>();
            IList<IWebElement> mainImageParentTable = null!;
            IWebElement imgPopup = null!;

            // Получаю контейнер с картинками
            try
            {
                mainImageParentTable = _driver.FindElements(By.CssSelector("div.sr-table__row.offer.sr-table__row_mx"));
            }
            catch (Exception) { }

            // Получаю все картинки thumbs
            try
            {
                // Находим все img элементы в li элементах с data-type='thumb'
                IWebElement? itemProductsTd = mainImageParentTable[0]?.FindElement(By.CssSelector("div.sr-table__td.td_product"));

                IWebElement? itemProductIcon = itemProductsTd?.FindElement(By.ClassName("mx-sr-item__img"));


                if (itemProductIcon is not null)
                {
                    itemProductIcon.Click();
                    await Task.Delay(1000);

                    try
                    {
                        imgPopup = _driver.FindElement(By.CssSelector("div.slider__item.img-viewer__slider-item img"));
                    }
                    catch (Exception)
                    {

                    }

                    string imagePath = imgPopup.GetAttribute("src");
                    images.Add(imagePath);
                }
            }
            catch (Exception)
            {

            }

            if (images.Count != 0)
                downloadedImages = await ImagesProcessAsync(images);

            return downloadedImages;
        }

        // Метод для формирования Url поискового запроса
        public string BuildUrl()
        {
            string url = $"{SearchPageUrl}{Articul}";

            return url;
        }

        // Инициализация драйвера
        private void InitializeDriver()
        {
            UndetectDriver webDriver = new(_tempProfilePath);
            _driver = webDriver.GetDriver();
        }

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
        #endregion

    }
}
