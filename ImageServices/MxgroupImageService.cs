using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Threading;

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

        public MxgroupImageService()
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
            Thread.Sleep(1000);

            try
            {
                try
                {
                    IWebElement logInput = _driver.FindElement(By.Name("username"));
                    Actions builder = new Actions(_driver);

                    builder.MoveToElement(logInput)
                           .Click()
                           .SendKeys(UserName)
                           .Build()
                           .Perform();

                    Thread.Sleep(1000);
                }
                catch (Exception) { }

                try
                {
                    IWebElement passInput = _driver.FindElement(By.Name("password"));

                    Thread.Sleep(1000);

                    passInput.SendKeys(Password);
                }
                catch (Exception) { }

                try
                {
                    IWebElement sumbitBtn = _driver.FindElement(By.ClassName("btn"));

                    sumbitBtn.Click();

                    Thread.Sleep(5000);
                }
                catch (Exception) { }


                WaitingReadyStatePage();

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
            Thread.Sleep(200);
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
                    Thread.Sleep(1000);

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
        protected override void SpecificRunAsync(string brandName, string articul)
        {

        }

        protected override void CloseDriver()
        {
            _driver.Close();
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
                    Thread.Sleep(1000);

                    try
                    {
                        imgPopup = _driver.FindElement(By.CssSelector("div.slider__item.img-viewer__slider-item img"));
                    }
                    catch (Exception)
                    {

                    }
                    //< h1 >< span class="mx-text_up mx-text_light mx-text_sm">Результат поиска</span>SS-3033</h1>
                    //<div class="text">…подождите, идет поиск&nbsp;<svg class="icon spinner mx-spin"><use xlink:href="/images/sprite.svg#icon-spinner2"></use></svg></div>

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
            UndetectDriver webDriver = new();
            _driver = webDriver.GetDriver();
        }

        private async Task<List<string>> ImagesProcessAsync(List<string> images)
        {
            List<string> downloadedImages = new();

            // Проверяю создан ли путь для хранения картинок
            FolderManager folderManager = new();
            bool folderContainsFiles = folderManager.ArticulFolderContainsFiles(brand: Brand, articul: Articul, out _imagesLocalPath);

            Thread.Sleep(1000);

            if (!folderContainsFiles)
            {
                // Скачиваю изображения
                ImageDownloader? downloader = new(Articul, _imagesLocalPath, images);
                downloadedImages = await downloader.DownloadImagesAsync();
            }

            return downloadedImages;
        }

        public void WaitingReadyStatePage()
        {
            while (true)
            {
                try
                {

                    IWebElement spinner = _driver.FindElement(By.CssSelector("div.alert.success"));

                    continue;

                }
                catch (Exception)
                {
                    break;
                }
            }
        }
        #endregion

    }
}
